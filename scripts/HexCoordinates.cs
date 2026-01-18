using Godot;

namespace MilSandbox.Scripts;

using MilSandbox.Scripts.Autoload;

/// <summary>
/// Hex grid coordinate system utilities
/// Handles position calculation, neighbor finding, and world wrapping
/// </summary>
public static class HexCoordinates
{
	/// <summary>
	/// Calculate 3D world position from grid coordinates
	/// </summary>
	public static Vector3 CalculatePosition(int gridX, int gridY)
	{
		var xPos = (Autoload.GameConstants.HEXAGON_WIDTH * gridX * 1.5f + 
					Autoload.GameConstants.SECTOR_WIDTH * (gridY % 2)) + Autoload.GameConstants.MAP_X;
		var yPos = (Autoload.GameConstants.HEXAGON_HEIGHT * gridY / 2.0f) + Autoload.GameConstants.MAP_Y;
		return new Vector3(xPos, Autoload.GameConstants.MAP_Y_LEVEL, yPos);
	}

	/// <summary>
	/// Get neighbor offsets based on whether row is even or odd
	/// Returns array of [x_offset, y_offset] pairs for the 6 neighbors
	/// </summary>
	public static int[][] GetNeighborOffsets(int gridY)
	{
		if (gridY % 2 == 0)
		{
			// Even row offsets
			return new[]
			{
				new[] { 0, -2 }, new[] { 0, -1 }, new[] { 0, 1 },
				new[] { 0, 2 }, new[] { -1, 1 }, new[] { -1, -1 }
			};
		}
		else
		{
			// Odd row offsets
			return new[]
			{
				new[] { 0, -2 }, new[] { 1, -1 }, new[] { 1, 1 },
				new[] { 0, 2 }, new[] { 0, 1 }, new[] { 0, -1 }
			};
		}
	}

	/// <summary>
	/// Handle world-edge wrapping for x-coordinate
	/// Map wraps horizontally (like a cylinder)
	/// </summary>
	public static int WrapXCoordinate(int x)
	{
		if (x < 0)
		{
			return Autoload.GameConstants.GRID_SIZE_X - 1;
		}
		
		if (x >= Autoload.GameConstants.GRID_SIZE_X)
		{
			return 0;
		}
		
		return x;
	}

	/// <summary>
	/// Check if coordinates are within grid bounds (after wrapping x)
	/// </summary>
	public static bool IsValidCoordinate(int x, int y)
	{
		// X is always valid due to wrapping
		// Only check Y bounds
		return y >= 0 && y < Autoload.GameConstants.GRID_SIZE_Y;
	}

	/// <summary>
	/// Calculate distance between two hex tiles (simplified)
	/// </summary>
	public static int HexDistance(int x1, int y1, int x2, int y2)
	{
		// Simple Manhattan distance approximation
		// For proper hex distance, would need to convert to cube coordinates
		return Mathf.Abs(x1 - x2) + Mathf.Abs(y1 - y2);
	}
}
