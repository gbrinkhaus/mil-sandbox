using Godot;

namespace MilSandbox.Scripts;

/// <summary>
/// Camera controller for map navigation
/// WASD - Pan camera
/// Mouse scroll - Zoom in/out
/// </summary>
public partial class CameraController : Camera3D
{
	// FOV constraints
	private const int MIN_FOV = 10;
	private const int MAX_FOV = 90;
	
	// Keyboard panning
	private const float KEYBOARD_ACCELERATION = 60f;
	private const float KEYBOARD_MAX_SPEED = 30f;
	private const float KEYBOARD_DECELERATION = 100f;
	
	// Mouse panning
	private const float MOUSE_PAN_SPEED = 0.1f;
	private const float MOUSE_PAN_ACCELERATION = 500f;
	private const float MOUSE_PAN_MAX_SPEED = 500f;
	private const float MOUSE_PAN_DECELERATION = 2000f;
	
	// Mouse rotation
	private const float MOUSE_ROTATE_SPEED = 0.01f;
	private const float MOUSE_ROTATE_ACCELERATION = 200f;
	private const float MOUSE_ROTATE_MAX_SPEED = 50f;
	private const float MOUSE_ROTATE_DECELERATION = 1000f;
	
	// Mouse zoom
	private const float MOUSE_ZOOM_ACCELERATION = 5000f;
	private const float MOUSE_ZOOM_MAX_SPEED = 3000f;
	private const float MOUSE_ZOOM_DECELERATION = 20000f;
	
	private bool middleMousePressed = false;
	private bool rightMousePressed = false;
	private Vector2 lastMousePos = Vector2.Zero;
	private Vector2 mouseMotionDelta = Vector2.Zero;  // Accumulated mouse motion delta
	private Vector3 keyboardVelocity = Vector3.Zero;  // Current keyboard movement velocity
	private Vector3 mousePanVelocity = Vector3.Zero;  // Current mouse pan velocity
	private Vector3 mouseRotateVelocity = Vector3.Zero;  // Current mouse rotation velocity
	private float mouseZoomVelocity = 0f;  // Current mouse zoom velocity
	private int mouseZoomDirection = 0;  // 1 for zoom in, -1 for zoom out, 0 for none

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
		// KEYBOARD MOVEMENT
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
			// Accelerate velocity toward max speed
			input = input.Normalized();
			var targetVelocity = input * KEYBOARD_MAX_SPEED;
			keyboardVelocity = keyboardVelocity.Lerp(targetVelocity, KEYBOARD_ACCELERATION * delta / KEYBOARD_MAX_SPEED);
		}
		else
		{
			// Decelerate velocity when no keys pressed
			keyboardVelocity = keyboardVelocity.Lerp(Vector3.Zero, KEYBOARD_DECELERATION * delta / KEYBOARD_MAX_SPEED);
		}

		// Apply keyboard velocity to position
		Position += keyboardVelocity * delta;

		// MOUSE MOVEMENT
		if (middleMousePressed && mouseMotionDelta != Vector2.Zero)
		{
			// Accelerate pan velocity
			var targetPanDir = new Vector3(-mouseMotionDelta.X, 0, -mouseMotionDelta.Y).Normalized();
			var targetPanVel = targetPanDir * MOUSE_PAN_MAX_SPEED;
			mousePanVelocity = mousePanVelocity.Lerp(targetPanVel, MOUSE_PAN_ACCELERATION * delta / MOUSE_PAN_MAX_SPEED);
			mouseMotionDelta = Vector2.Zero;
		}
		else if (!middleMousePressed)
		{
			// Decelerate pan velocity
			mousePanVelocity = mousePanVelocity.Lerp(Vector3.Zero, MOUSE_PAN_DECELERATION * delta / MOUSE_PAN_MAX_SPEED);
		}
		Position += mousePanVelocity * MOUSE_PAN_SPEED * delta;

		if (rightMousePressed && mouseMotionDelta != Vector2.Zero)
		{
			// Accelerate rotation velocity
			var targetRotDir = new Vector3(-mouseMotionDelta.Y, -mouseMotionDelta.X, 0);
			var targetRotVel = targetRotDir * MOUSE_ROTATE_MAX_SPEED;
			mouseRotateVelocity = mouseRotateVelocity.Lerp(targetRotVel, MOUSE_ROTATE_ACCELERATION * delta / MOUSE_ROTATE_MAX_SPEED);
			mouseMotionDelta = Vector2.Zero;
		}
		else if (!rightMousePressed)
		{
			// Decelerate rotation velocity
			mouseRotateVelocity = mouseRotateVelocity.Lerp(Vector3.Zero, MOUSE_ROTATE_DECELERATION * delta / MOUSE_ROTATE_MAX_SPEED);
		}
		Rotation += mouseRotateVelocity * MOUSE_ROTATE_SPEED * delta;

		// MOUSE ZOOM
		if (mouseZoomDirection != 0)
		{
			// Accelerate zoom velocity
			var targetZoomVel = mouseZoomDirection * MOUSE_ZOOM_MAX_SPEED;
			mouseZoomVelocity = Mathf.Lerp(mouseZoomVelocity, targetZoomVel, MOUSE_ZOOM_ACCELERATION * delta / MOUSE_ZOOM_MAX_SPEED);
		}
		else
		{
			// Decelerate zoom velocity
			mouseZoomVelocity = Mathf.Lerp(mouseZoomVelocity, 0f, MOUSE_ZOOM_DECELERATION * delta / MOUSE_ZOOM_MAX_SPEED);
		}
		Fov += mouseZoomVelocity * delta;
		Fov = Mathf.Clamp(Fov, MIN_FOV, MAX_FOV);
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
				mouseZoomDirection = -1;  // Zoom in (decrease FOV)
				GetTree().Root.SetInputAsHandled();
			}
			else if (mouseEvent.ButtonIndex == MouseButton.WheelUp && !mouseEvent.Pressed)
			{
				mouseZoomDirection = 0;
			}
			else if (mouseEvent.ButtonIndex == MouseButton.WheelDown && mouseEvent.Pressed)
			{
				mouseZoomDirection = 1;  // Zoom out (increase FOV)
				GetTree().Root.SetInputAsHandled();
			}
			else if (mouseEvent.ButtonIndex == MouseButton.WheelDown && !mouseEvent.Pressed)
			{
				mouseZoomDirection = 0;
			}
		}
		else if (@event is InputEventMouseMotion mouseMotion)
		{
			var delta = mouseMotion.Position - lastMousePos;
			
			if (middleMousePressed || rightMousePressed)
			{
				mouseMotionDelta += delta;
				GetTree().Root.SetInputAsHandled();
			}
			
			lastMousePos = mouseMotion.Position;
		}
	}
}
