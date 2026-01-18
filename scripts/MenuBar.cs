using Godot;

namespace MilSandbox.Scripts;

public partial class MenuBar : PanelContainer
{
	private Label debugLabel;

	public override void _Ready()
	{
		// Setup as CanvasLayer for UI rendering
		var canvasLayer = new CanvasLayer();
		GetParent().AddChild(canvasLayer);
		GetParent().MoveChild(this, GetParent().GetChildCount() - 1);

		// Set anchors and grow to fill full width
		AnchorLeft = 0.0f;
		AnchorRight = 1.0f;
		AnchorTop = 0.0f;
		AnchorBottom = 0.0f;
		OffsetBottom = 30;
		GrowHorizontal = Control.GrowDirection.Both;

		// Create horizontal box for single-line layout
		var hboxContainer = new HBoxContainer();
		hboxContainer.GrowHorizontal = Control.GrowDirection.Both;
		hboxContainer.GrowVertical = Control.GrowDirection.Both;
		AddChild(hboxContainer);

		// Debug info label (all on one line)
		debugLabel = new Label 
		{ 
			Text = "FPS: 0 | Cam Pos: (0, 0, 0) | Rot: (0, 0, 0) | FOV: 0",
			CustomMinimumSize = new Vector2(0, 24)
		};
		hboxContainer.AddChild(debugLabel);

		// Setup panel styling
		CustomMinimumSize = new Vector2(0, 30);

		GD.Print("✓ MenuBar initialized");
	}

	public override void _Process(double delta)
	{
		var camera = GetViewport().GetCamera3D();
		if (camera != null)
		{
			var pos = camera.GlobalPosition;
			var rot = camera.Rotation;
			var fov = camera.Fov;
			debugLabel.Text = $"FPS: {Engine.GetFramesPerSecond()} | Pos: ({pos.X:F1}, {pos.Y:F1}, {pos.Z:F1}) | Rot: ({rot.X:F2}, {rot.Y:F2}, {rot.Z:F2}) | FOV: {fov:F1}";
		}
	}
}

