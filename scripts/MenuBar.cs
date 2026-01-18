using Godot;

namespace MilSandbox.Scripts;

public partial class MenuBar : PanelContainer
{
	private Label debugLabel;
	private WorldMap worldMap;
	private OptionButton terrainTypeDropdown;
	private OptionButton countryDropdown;
	private bool updatingUI = false;

	public override void _Ready()
	{
		GD.Print("MenuBar._Ready starting");
		
		// Setup as CanvasLayer for UI rendering
		var canvasLayer = new CanvasLayer();
		GetParent().AddChild(canvasLayer);
		GetParent().MoveChild(this, GetParent().GetChildCount() - 1);

		// Set anchors and grow to fill full width
		AnchorLeft = 0.0f;
		AnchorRight = 1.0f;
		AnchorTop = 0.0f;
		AnchorBottom = 0.0f;
		OffsetBottom = 50;
		GrowHorizontal = Control.GrowDirection.Both;

		// Create horizontal box for single-line layout
		var hboxContainer = new HBoxContainer();
		hboxContainer.GrowHorizontal = Control.GrowDirection.Both;
		AddChild(hboxContainer);

		// Debug info label (left side)
		debugLabel = new Label 
		{ 
			Text = "FPS: 0 | Cam Pos: (0, 0, 0) | Rot: (0, 0, 0) | FOV: 0",
			CustomMinimumSize = new Vector2(0, 24),
			SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
		};
		hboxContainer.AddChild(debugLabel);

		// Spacer
		var spacer = new Control();
		spacer.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		hboxContainer.AddChild(spacer);

		// Right side controls
		var rightBox = new HBoxContainer();
		rightBox.CustomMinimumSize = new Vector2(300, 24);
		hboxContainer.AddChild(rightBox);

		// Terrain type dropdown
		terrainTypeDropdown = new OptionButton();
		foreach (var terrainName in Autoload.GameConstants.FIELD_NAMES)
		{
			terrainTypeDropdown.AddItem(terrainName);
		}
		terrainTypeDropdown.CustomMinimumSize = new Vector2(120, 24);
		terrainTypeDropdown.ItemSelected += OnTerrainTypeChanged;
		rightBox.AddChild(terrainTypeDropdown);

		// Country dropdown
		countryDropdown = new OptionButton();
		countryDropdown.CustomMinimumSize = new Vector2(150, 24);
		countryDropdown.ItemSelected += OnCountryChanged;
		rightBox.AddChild(countryDropdown);

		// Setup panel styling
		CustomMinimumSize = new Vector2(0, 50);

		GD.Print("✓ MenuBar initialized");
	}

	public override void _Input(InputEvent @event)
	{
		// Consume all input that happens over the menu bar
		if (@event is InputEventMouseButton mouseEvent && GetGlobalRect().HasPoint(GetViewport().GetMousePosition()))
		{
			GetTree().Root.SetInputAsHandled();
		}
	}

	public override void _Process(double delta)
	{
		var camera = GetViewport().GetCamera3D();
		var debugParts = new System.Collections.Generic.List<string>();
		
		// FPS
		debugParts.Add($"FPS: {Engine.GetFramesPerSecond()}");
		
		// Camera info
		if (camera != null)
		{
			var pos = camera.GlobalPosition;
			var rot = camera.Rotation;
			var fov = camera.Fov;
			debugParts.Add($"Pos: ({pos.X:F1}, {pos.Y:F1}, {pos.Z:F1})");
			debugParts.Add($"Rot: ({rot.X:F2}, {rot.Y:F2}, {rot.Z:F2})");
			debugParts.Add($"FOV: {fov:F1}");
		}
		
		// Selected tile info
		worldMap = GameManager.CurrentWorldMap;
		if (worldMap != null)
		{
			// Populate country dropdown once if empty
			if (countryDropdown.ItemCount == 0)
			{
				worldMap.PopulateCountryDropdown(countryDropdown);
			}
			
			if (!string.IsNullOrEmpty(worldMap.SelectedCountry))
			{
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
			else
			{
				if (!updatingUI)
				{
					updatingUI = true;
					terrainTypeDropdown.Selected = -1;
					countryDropdown.Selected = -1;
					updatingUI = false;
				}
			}
		}
		
		debugLabel.Text = string.Join(" | ", debugParts);
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

