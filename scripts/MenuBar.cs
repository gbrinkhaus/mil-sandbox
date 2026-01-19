using Godot;

namespace MilSandbox.Scripts;

public partial class MenuBar : PanelContainer
{
	private Label debugLabel;
	private Label tileInfoLabel;
	private WorldMap worldMap;
	private OptionButton terrainTypeDropdown;
	private OptionButton countryDropdown;
	private bool updatingUI = false;

	public override void _Ready()
	{
		GD.Print("MenuBar._Ready starting");
		
		// Setup PanelContainer to fill top of screen
		AnchorLeft = 0f;
		AnchorTop = 0f;
		AnchorRight = 1f;
		AnchorBottom = 0f;
		OffsetTop = 0;
		OffsetBottom = 50;
		CustomMinimumSize = new Vector2(0, 50);
		
		// Main horizontal layout
		var mainBox = new HBoxContainer();
		mainBox.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		mainBox.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
		AddChild(mainBox);

		// Left: Debug label (camera info)
		debugLabel = new Label 
		{ 
			Text = "Debug Info",
			SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
			SizeFlagsVertical = Control.SizeFlags.ShrinkCenter,
			CustomMinimumSize = new Vector2(0, 20)
		};
		mainBox.AddChild(debugLabel);

		// Spacer (takes remaining space, naturally pushes children after it to the right)
		var spacer = new Control();
		spacer.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		mainBox.AddChild(spacer);

		// Tile info label (right side, no special flags)
		tileInfoLabel = new Label
		{
			Text = "Tile: none",
			SizeFlagsVertical = Control.SizeFlags.ShrinkCenter,
			CustomMinimumSize = new Vector2(250, 20)
		};
		mainBox.AddChild(tileInfoLabel);

		// Terrain dropdown (right side)
		terrainTypeDropdown = new OptionButton();
		terrainTypeDropdown.CustomMinimumSize = new Vector2(100, 24);
		terrainTypeDropdown.SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;
		terrainTypeDropdown.Show();
		foreach (var name in Autoload.GameConstants.FIELD_NAMES)
			terrainTypeDropdown.AddItem(name);
		terrainTypeDropdown.ItemSelected += OnTerrainTypeChanged;
		mainBox.AddChild(terrainTypeDropdown);
		GD.Print("✓ Terrain dropdown added");

		// Country dropdown (right side)
		countryDropdown = new OptionButton();
		countryDropdown.CustomMinimumSize = new Vector2(100, 24);
		countryDropdown.SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;
		countryDropdown.Show();
		countryDropdown.ItemSelected += OnCountryChanged;
		mainBox.AddChild(countryDropdown);
		GD.Print("✓ Country dropdown added");

		GD.Print("✓ MenuBar layout complete");
	}

	public override void _Process(double delta)
	{
		var camera = GetViewport().GetCamera3D();
		var cameraParts = new System.Collections.Generic.List<string>();
		var tileParts = new System.Collections.Generic.List<string>();
		
		// Camera info (left)
		cameraParts.Add($"FPS: {Engine.GetFramesPerSecond()}");
		if (camera != null)
		{
			var pos = camera.GlobalPosition;
			var rot = camera.Rotation;
			var fov = camera.Fov;
			cameraParts.Add($"Pos: ({pos.X:F1}, {pos.Y:F1}, {pos.Z:F1})");
			cameraParts.Add($"Rot: ({rot.X:F2}, {rot.Y:F2}, {rot.Z:F2})");
			cameraParts.Add($"FOV: {fov:F1}");
		}
		
		// Selected tile info (right)
		worldMap = GameManager.CurrentWorldMap;
		if (worldMap != null)
		{
			// Populate country dropdown once if empty
			if (countryDropdown.ItemCount == 0)
			{
				worldMap.PopulateCountryDropdown(countryDropdown);
			}
			
			// Show tile info if a tile is selected
			if (worldMap.SelectedGridX >= 0 && worldMap.SelectedGridY >= 0)
			{
				var terrainName = Autoload.GameConstants.GetTerrainName(worldMap.SelectedTerrainType);
				tileParts.Add($"Tile: ({worldMap.SelectedGridX}, {worldMap.SelectedGridY})");
				tileParts.Add($"Terrain: {terrainName}");
				if (!string.IsNullOrEmpty(worldMap.SelectedCountry))
					tileParts.Add($"Country: {worldMap.SelectedCountry}");
				
				// Update UI fields if not currently being edited
				if (!updatingUI)
				{
					updatingUI = true;
					terrainTypeDropdown.Selected = worldMap.SelectedTerrainType;
					
					// Find country in dropdown
					for (int i = 0; i < countryDropdown.ItemCount; i++)
					{
						if (countryDropdown.GetItemText(i) == worldMap.SelectedCountry)
						{
							countryDropdown.Selected = i;
							break;
						}
					}
					updatingUI = false;
				}
			}
		}
		
		debugLabel.Text = string.Join(" | ", cameraParts);
		tileInfoLabel.Text = tileParts.Count > 0 ? string.Join(" | ", tileParts) : "Tile: none";
	}

	private void OnTerrainTypeChanged(long index)
	{
		if (updatingUI || worldMap == null || worldMap.SelectedGridX < 0) return;
		
		// TODO: Update tile terrain type
		GD.Print($"Terrain type changed to {index}");
	}

	private void OnCountryChanged(long index)
	{
		if (updatingUI || worldMap == null || worldMap.SelectedGridX < 0) return;
		
		var newCountry = countryDropdown.GetItemText((int)index);
		// TODO: Update tile country
		GD.Print($"Country changed to {newCountry}");
	}
}

