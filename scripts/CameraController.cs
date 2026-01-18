using Godot;

namespace MilSandbox.Scripts;

/// <summary>
/// Camera controller for map navigation
/// WASD - Pan camera
/// Mouse scroll - Zoom in/out
/// </summary>
public partial class CameraController : Camera3D
{
	private const float PAN_SPEED = 20f;
	private const float ZOOM_SPEED = 5f;
	private const float MIN_FOV = 10f;
	private const float MAX_FOV = 90f;
	private const float MOUSE_PAN_SPEED = 0.1f;
	private const float MOUSE_ROTATE_SPEED = 0.01f;
	
	private bool middleMousePressed = false;
	private bool rightMousePressed = false;
	private Vector2 lastMousePos = Vector2.Zero;

	public override void _Ready()
	{
		GD.Print("✓ CameraController attached to camera");
	}

	public override void _Process(double delta)
	{
		HandlePanning((float)delta);
	}

	private void HandlePanning(float delta)
	{
		var input = Vector3.Zero;

		if (Input.IsKeyPressed(Key.W) || Input.IsKeyPressed(Key.Up))
			input.Z -= 1;
		if (Input.IsKeyPressed(Key.S) || Input.IsKeyPressed(Key.Down))
			input.Z += 1;
		if (Input.IsKeyPressed(Key.A) || Input.IsKeyPressed(Key.Left))
			input.X -= 1;
		if (Input.IsKeyPressed(Key.D) || Input.IsKeyPressed(Key.Right))
			input.X += 1;

		if (input != Vector3.Zero)
		{
			Position += input.Normalized() * PAN_SPEED * delta;
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent)
		{
			if (mouseEvent.ButtonIndex == MouseButton.Middle)
			{
				middleMousePressed = mouseEvent.Pressed;
				lastMousePos = GetViewport().GetMousePosition();
				GetTree().Root.SetInputAsHandled();
			}
			else if (mouseEvent.ButtonIndex == MouseButton.Right)
			{
				rightMousePressed = mouseEvent.Pressed;
				lastMousePos = GetViewport().GetMousePosition();
				GetTree().Root.SetInputAsHandled();
			}
			else if (mouseEvent.ButtonIndex == MouseButton.WheelUp && mouseEvent.Pressed)
			{
				Fov = Mathf.Max(Fov - ZOOM_SPEED, MIN_FOV);
				GetTree().Root.SetInputAsHandled();
			}
			else if (mouseEvent.ButtonIndex == MouseButton.WheelDown && mouseEvent.Pressed)
			{
				Fov = Mathf.Min(Fov + ZOOM_SPEED, MAX_FOV);
				GetTree().Root.SetInputAsHandled();
			}
		}
		else if (@event is InputEventMouseMotion mouseMotion)
		{
			var delta = mouseMotion.Position - lastMousePos;
			
			if (middleMousePressed)
			{
				// Middle mouse: pan X/Z
				var newPos = Position;
				newPos.X -= delta.X * MOUSE_PAN_SPEED;
				newPos.Z -= delta.Y * MOUSE_PAN_SPEED;
				Position = newPos;
				GetTree().Root.SetInputAsHandled();
			}
			else if (rightMousePressed)
			{
				// Right mouse: rotate camera angle
				var rotation = Rotation;
				rotation.Y -= delta.X * MOUSE_ROTATE_SPEED;  // Left/right yaw
				rotation.X -= delta.Y * MOUSE_ROTATE_SPEED;  // Up/down pitch
				Rotation = rotation;
				GetTree().Root.SetInputAsHandled();
			}
			
			lastMousePos = mouseMotion.Position;
		}
	}
}
