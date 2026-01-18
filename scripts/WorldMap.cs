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
	private Dictionary<int, Material> terrainMaterials = new();
	private Node3D tileContainer;

	public override void _Ready()
	{
		GD.Print("=== WorldMap Ready ===");
		
		// Create container for tiles
		tileContainer = new Node3D { Name = "TileContainer" };
		AddChild(tileContainer);

		// Initialize terrain materials (placeholder - can be expanded)
		InitializeTerrainMaterials();

		// Load and generate map
		LoadMapData();
	}

	private void InitializeTerrainMaterials()
	{
		// Create basic materials for each terrain type
		// 0: grass, 1: forest, 2: desert, 3: sea, 4: mountain, 5: city, 6: snow, 7: jungle, 8: seaice
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

		for (int i = 0; i < terrainColors.Length; i++)
		{
			var material = new StandardMaterial3D
			{
				AlbedoColor = terrainColors[i],
				RoughnessTextureChannel = BaseMaterial3D.TextureChannel.Red
			};
			terrainMaterials[i] = material;
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
					tileCount++;
				}
				GD.Print($"✓ Created {tileCount} hex tiles");
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

			// Create visual hex mesh
			var meshInstance = new MeshInstance3D();
			var hexMesh = CreateHexMesh();
			meshInstance.Mesh = hexMesh;

			// Apply terrain material
			if (terrainMaterials.TryGetValue(terrainType, out var material))
			{
				meshInstance.SetSurfaceOverrideMaterial(0, material);
			}

			// Position hex tile
			var position = HexCoordinates.CalculatePosition(x, y);
			meshInstance.Position = position;

			// Store metadata
			meshInstance.SetMeta("grid_x", x);
			meshInstance.SetMeta("grid_y", y);
			meshInstance.SetMeta("terrain_type", terrainType);
			meshInstance.SetMeta("country", country);
			meshInstance.SetMeta("city", city);
			meshInstance.SetMeta("power", power);
			meshInstance.SetMeta("industry", industry);

			meshInstance.Name = $"Hex_{x}_{y}";

			// Add collider for interactions
			var collider = new StaticBody3D();
			var collisionShape = new CollisionShape3D();
			collisionShape.Shape = new BoxShape3D { Size = new Vector3(0.8f, 0.2f, 0.8f) };
			collider.AddChild(collisionShape);
			meshInstance.AddChild(collider);

			// Add to container
			tileContainer.AddChild(meshInstance);
		}
		catch (Exception e)
		{
			GD.PrintErr($"Error creating hex tile: {e.Message}");
		}
	}

	private Mesh CreateHexMesh()
	{
		var meshBuilder = new MeshBuilder();
		const float size = 0.5f;
		const float height = 0.1f;

		// Create hex shape in XZ plane
		Vector3[] vertices = new Vector3[7];
		vertices[0] = new Vector3(0, 0, 0); // center

		for (int i = 0; i < 6; i++)
		{
			float angle = (i * 60f) * Mathf.Pi / 180f;
			vertices[i + 1] = new Vector3(
				Mathf.Cos(angle) * size,
				0,
				Mathf.Sin(angle) * size
			);
		}

		var mesh = new Mesh();
		// For now, create a simple box placeholder
		// TODO: Create proper hex geometry
		return new BoxMesh { Size = new Vector3(0.8f, 0.1f, 0.8f) };
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
