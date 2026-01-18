using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace MilSandbox.Scripts;

using MilSandbox.Scripts.Autoload;

/// <summary>
/// Generates and manages the world map from map_data.json
/// Creates hex tiles for each grid cell with terrain information
/// </summary>
public partial class WorldMap : Node3D
{
	private const string MAP_DATA_PATH = "res://data/map_data.json";
	private const string HEX_MODEL_PATH = "res://models/3d_hex.obj";
	private Dictionary<int, Material> terrainMaterials = new();
	private Dictionary<string, Material> countryHighlightMaterials = new();
	private Node3D tileContainer;
	private Mesh hexMesh;
	private string selectedCountry = "";
	private List<StaticBody3D> highlightedTiles = new();
	
	// Track selected tile for UI display
	public string SelectedCountry { get; private set; } = "";
	public int SelectedTerrainType { get; private set; } = -1;
	public int SelectedGridX { get; private set; } = -1;
	public int SelectedGridY { get; private set; } = -1;
	
	private HashSet<string> allCountries = new();

	public override void _Ready()
	{
		// Register with GameManager
		GameManager.CurrentWorldMap = this;
		
		GD.Print("=== WorldMap Ready ===");
		
		// Load hex mesh model
		LoadHexMesh();
		
		// Create container for tiles
		tileContainer = new Node3D { Name = "TileContainer" };
		AddChild(tileContainer);

		// Initialize terrain materials (placeholder - can be expanded)
		InitializeTerrainMaterials();

		// Load and generate map
		LoadMapData();
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left && mouseEvent.Pressed)
		{
			GD.Print($"CLICK DETECTED at mouse pos: {GetViewport().GetMousePosition()}");
			HandleTileClick();
			GetTree().Root.SetInputAsHandled();
		}
	}

	private void HandleTileClick()
	{
		var camera = GetViewport().GetCamera3D();
		if (camera == null) 
		{
			GD.Print("ERROR: Camera is null!");
			return;
		}

		var mousePos = GetViewport().GetMousePosition();
		var ray = camera.ProjectRayOrigin(mousePos);
		var rayNormal = camera.ProjectRayNormal(mousePos);

		GD.Print($"Raycast from {ray} in direction {rayNormal}");

		var query = PhysicsRayQueryParameters3D.Create(ray, ray + rayNormal * 1000f);
		query.CollisionMask = 1;  // Only detect layer 1 (where tiles are)
		
		var space = GetWorld3D().DirectSpaceState;
		var result = space.IntersectRay(query);

		GD.Print($"Raycast result count: {result.Count}");

		if (result.Count > 0)
		{
			if (result.TryGetValue("collider", out var colliderObj))
			{
				if (colliderObj.Obj is StaticBody3D tile)
				{
					if (tile.HasMeta("country"))
					{
						var country = tile.GetMeta("country", "").AsString();
						var terrainType = tile.GetMeta("terrain_type", -1).AsInt32();
						var gridX = tile.GetMeta("grid_x", -1).AsInt32();
						var gridY = tile.GetMeta("grid_y", -1).AsInt32();
						if (!string.IsNullOrEmpty(country))
						{
							SelectedCountry = country;
							SelectedTerrainType = terrainType;
							SelectedGridX = gridX;
							SelectedGridY = gridY;
							HighlightCountry(country);
						}
					}
				}
			}
		}
		else
		{
			GD.Print("Raycast returned no results!");
		}
	}

	private void HighlightCountry(string country)
	{
		// Clear previous highlights
		ClearHighlights();

		selectedCountry = country;

		// Find all tiles with this country
		foreach (Node child in tileContainer.GetChildren())
		{
			if (child is StaticBody3D tile && tile.HasMeta("country"))
			{
				var tileCountry = tile.GetMeta("country", "").AsString();
				if (tileCountry == country)
				{
					highlightedTiles.Add(tile);
					
					// Get the MeshInstance3D child (first child at index 0)
					var meshChild = tile.GetChild(0) as MeshInstance3D;
					if (meshChild != null)
					{
						// Get the original material and darken it
						var terrainType = tile.GetMeta("terrain_type", 0).AsInt32();
						if (terrainMaterials.TryGetValue(terrainType, out var originalMat) && originalMat is StandardMaterial3D stdMat)
						{
							// Create a darker version by blending with dark gray (20% dark gray)
							var darkenedColor = new Color(
								Mathf.Lerp(stdMat.AlbedoColor.R, 0.2f, 0.2f),
								Mathf.Lerp(stdMat.AlbedoColor.G, 0.2f, 0.2f),
								Mathf.Lerp(stdMat.AlbedoColor.B, 0.2f, 0.2f),
								stdMat.AlbedoColor.A
							);
							
							var highlightMat = new StandardMaterial3D
							{
								AlbedoTexture = stdMat.AlbedoTexture,
								AlbedoColor = darkenedColor,
								Roughness = stdMat.Roughness
							};
							
							meshChild.SetSurfaceOverrideMaterial(0, highlightMat);
						}
					}
				}
			}
		}
	}

	private void ClearHighlights()
	{
		foreach (var tile in highlightedTiles)
		{
			if (tile != null && tile.HasMeta("terrain_type"))
			{
				var meshChild = tile.GetChild(0) as MeshInstance3D;
				if (meshChild != null)
				{
					var terrainType = tile.GetMeta("terrain_type", 0).AsInt32();
					if (terrainMaterials.TryGetValue(terrainType, out var material))
					{
						meshChild.SetSurfaceOverrideMaterial(0, material);
					}
				}
			}
		}
		highlightedTiles.Clear();
		selectedCountry = "";
	}

	private Material GetHighlightMaterial(string country)
	{
		if (countryHighlightMaterials.TryGetValue(country, out var material))
		{
			return material;
		}

		// Create a highlight material with dark gray tint
		var highlightMat = new StandardMaterial3D
		{
			AlbedoColor = new Color(0.2f, 0.2f, 0.2f, 1f),  // Dark gray
			Roughness = 0.6f
		};

		countryHighlightMaterials[country] = highlightMat;
		return highlightMat;
	}

	private void InitializeTerrainMaterials()
	{
		// Load textured materials for each terrain type with color tint
		// 0: grass, 1: forest, 2: desert, 3: sea, 4: mountain, 5: city, 6: snow, 7: jungle, 8: seaice
		var texturePaths = new string[]
		{
			"res://textures/hex_grass_map.png",      // 0: grass
			"res://textures/hex_forest_map.png",     // 1: forest
			"res://textures/hex_desert_map.png",     // 2: desert
			"res://textures/hex_sea_map.png",        // 3: sea
			"res://textures/hex_mountain_map.png",   // 4: mountain
			"res://textures/hex_city_map.png",       // 5: city
			"res://textures/hex_snow_map.png",       // 6: snow
			"res://textures/hex_jungle_map.png",     // 7: jungle
			"res://textures/hex_seaice_map.png"      // 8: seaice
		};

		var terrainColors = new Color[]
		{
			new Color(0.2f, 0.8f, 0.2f, 1f),  // grass - green
			new Color(0.1f, 0.5f, 0.1f, 1f),  // forest - dark green
			new Color(0.9f, 0.8f, 0.3f, 1f),  // desert - yellow
			new Color(0.2f, 0.5f, 0.9f, 1f),  // sea - blue
			new Color(0.6f, 0.6f, 0.6f, 1f),  // mountain - gray
			new Color(0.8f, 0.2f, 0.2f, 1f),  // city - red
			new Color(0.95f, 0.95f, 0.95f, 1f), // snow - white
			new Color(0.3f, 0.6f, 0.2f, 1f),  // jungle - dark green
			new Color(0.7f, 0.9f, 0.95f, 1f)  // seaice - light blue
		};

		for (int i = 0; i < texturePaths.Length; i++)
		{
			var texture = GD.Load<Texture2D>(texturePaths[i]);
			if (texture != null)
			{
				// Lighten the color for blending (20% visible underneath texture)
				var blendedColor = new Color(
					Mathf.Lerp(1f, terrainColors[i].R, 0.2f),
					Mathf.Lerp(1f, terrainColors[i].G, 0.2f),
					Mathf.Lerp(1f, terrainColors[i].B, 0.2f),
					1f
				);

				var material = new StandardMaterial3D
				{
					AlbedoTexture = texture,
					AlbedoColor = blendedColor,
					Roughness = 0.8f
				};
				terrainMaterials[i] = material;
				GD.Print($"  Loaded texture {i}: {texturePaths[i]}");
			}
			else
			{
				GD.PrintErr($"Failed to load texture: {texturePaths[i]}");
				// Fallback to color-based material
				var material = new StandardMaterial3D { AlbedoColor = terrainColors[i] };
				terrainMaterials[i] = material;
			}
		}

		GD.Print($"✓ Initialized {terrainMaterials.Count} terrain materials");
	}

	private void LoadMapData()
	{
		var file = FileAccess.Open(MAP_DATA_PATH, FileAccess.ModeFlags.Read);
		if (file == null)
		{
			GD.PrintErr($"Failed to load map data from {MAP_DATA_PATH}");
			return;
		}

		try
		{
			var jsonText = file.GetAsText();
			using var jsonDoc = JsonDocument.Parse(jsonText);
			var root = jsonDoc.RootElement;

			// Parse tiles array
			if (root.TryGetProperty("tiles", out var tilesElement) && tilesElement.ValueKind == JsonValueKind.Array)
			{
				int tileCount = 0;
				foreach (var tileElement in tilesElement.EnumerateArray())
				{
					CreateHexTile(tileElement);
					
					// Collect unique countries
					if (tileElement.TryGetProperty("country", out var countryProp))
					{
						var country = countryProp.GetString();
						if (!string.IsNullOrEmpty(country))
						{
							allCountries.Add(country);
						}
					}
					
					tileCount++;
					if (tileCount == 1)
					{
						// Debug first tile
						int x = tileElement.GetProperty("x").GetInt32();
						int y = tileElement.GetProperty("y").GetInt32();
						var pos = HexCoordinates.CalculatePosition(x, y);
						GD.Print($"  First tile at grid ({x}, {y}) -> world pos {pos}");
					}
				}
				GD.Print($"✓ Created {tileCount} hex tiles");
				GD.Print($"✓ Found {allCountries.Count} unique countries: {string.Join(", ", allCountries)}");
			}
			else
			{
				GD.PrintErr("No 'tiles' array found in map data");
			}
		}
		catch (Exception e)
		{
			GD.PrintErr($"Error parsing map data: {e.Message}");
		}
	}

	public void PopulateCountryDropdown(OptionButton dropdown)
	{
		dropdown.Clear();
		var countryList = new System.Collections.Generic.List<string>(allCountries);
		countryList.Sort();
		foreach (var country in countryList)
		{
			dropdown.AddItem(country);
		}
	}

	private void CreateHexTile(JsonElement tileData)
	{
		try
		{
			// Extract tile data
			int x = tileData.GetProperty("x").GetInt32();
			int y = tileData.GetProperty("y").GetInt32();
			int terrainType = tileData.GetProperty("type").GetInt32();
			string country = tileData.TryGetProperty("country", out var cProp) ? cProp.GetString() : "";
			string city = tileData.TryGetProperty("city", out var cyProp) ? cyProp.GetString() : "";
			int power = tileData.TryGetProperty("power", out var pProp) ? pProp.GetInt32() : 0;
			int industry = tileData.TryGetProperty("industry", out var iProp) ? iProp.GetInt32() : 0;

			// Position hex tile
			var position = HexCoordinates.CalculatePosition(x, y);

			// Create physics body for the hex tile using actual mesh geometry
			var staticBody = new StaticBody3D();
			staticBody.Position = position;
			staticBody.Name = $"Hex_{x}_{y}";

			// Store metadata on the physics body
			staticBody.SetMeta("grid_x", x);
			staticBody.SetMeta("grid_y", y);
			staticBody.SetMeta("terrain_type", terrainType);
			staticBody.SetMeta("country", country);
			staticBody.SetMeta("city", city);
			staticBody.SetMeta("power", power);
			staticBody.SetMeta("industry", industry);

			// Create visual hex mesh
			var meshInstance = new MeshInstance3D();
			meshInstance.Mesh = hexMesh;
			meshInstance.Scale = new Vector3(0.25f, 0.25f, 0.25f);

			// Apply terrain material
			if (terrainMaterials.TryGetValue(terrainType, out var material))
			{
				meshInstance.SetSurfaceOverrideMaterial(0, material);
			}

			// Add mesh as visual
			staticBody.AddChild(meshInstance);

			// Create collision shape using the actual hex mesh trimesh
			var collisionShape = new CollisionShape3D();
			var trimeshShape = hexMesh.CreateTrimeshShape();

			if (trimeshShape != null)
			{
				collisionShape.Shape = trimeshShape;
				collisionShape.Scale = new Vector3(0.25f, 0.25f, 0.25f);
			}

			staticBody.AddChild(collisionShape);

			// Add to container
			tileContainer.AddChild(staticBody);
		}
		catch (Exception e)
		{
			GD.PrintErr($"Error creating hex tile: {e.Message}");
		}
	}

	private void LoadHexMesh()
	{
		try
		{
			var resource = GD.Load(HEX_MODEL_PATH);
			if (resource is Mesh mesh)
			{
				hexMesh = mesh;
				GD.Print($"✓ Loaded hex mesh from {HEX_MODEL_PATH}");
				GD.Print($"  Mesh type: {mesh.GetType().Name}");
			}
			else
			{
				GD.PrintErr($"Failed to load hex mesh: resource type is {resource?.GetType().Name ?? "null"}, not a Mesh");
				hexMesh = new BoxMesh { Size = new Vector3(0.8f, 0.1f, 0.8f) };
			}
		}
		catch (Exception e)
		{
			GD.PrintErr($"Error loading hex mesh: {e.Message}");
			hexMesh = new BoxMesh { Size = new Vector3(0.8f, 0.1f, 0.8f) };
		}
	}
}

/// <summary>
/// Helper class for building custom meshes
/// </summary>
public class MeshBuilder
{
	public List<Vector3> Vertices { get; set; } = new();
	public List<int> Indices { get; set; } = new();
	public List<Vector3> Normals { get; set; } = new();
}
