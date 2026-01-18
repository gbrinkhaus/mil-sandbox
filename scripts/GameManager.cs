using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace MilSandbox.Scripts;

using MilSandbox.Scripts.Tools;

/// <summary>
/// Game constants - grid configuration and terrain data
/// </summary>
public static class GameConstants
{
	public const int GRID_SIZE_X = 46;
	public const int GRID_SIZE_Y = 92;
	public const float HEXAGON_WIDTH = 0.60f;
	public const float HEXAGON_HEIGHT = 0.52f;
	public const float SECTOR_WIDTH = HEXAGON_WIDTH * 0.75f;
	public const int MAP_X = -10;
	public const int MAP_Y = 5;
	public const float MAP_Y_LEVEL = 0.51f;

	public static readonly string[] FIELD_NAMES = [
		"grass", "forest", "desert", "sea", "mountain", "city", "snow", "jungle", "seaice"
	];
}

/// <summary>
/// Main game manager - runs on startup
/// Handles initialization and map loading
/// </summary>
public partial class GameManager : Node3D
{
	public override void _Ready()
	{
		GD.Print("=== Game Manager Ready ===");
		GD.Print("GameConstants loaded");
		GD.Print($"Grid size: {GameConstants.GRID_SIZE_X} x {GameConstants.GRID_SIZE_Y}");
		
		// Run map converter if map_data.json doesn't exist
		if (!FileAccess.FileExists("res://data/map_data.json"))
		{
			GD.Print("Map data not found, converting...");
			var converter = new MapConverter();
			converter.ConvertMapData();
		}
		else
		{
			GD.Print("✓ Map data already exists");
		}
	}
}
