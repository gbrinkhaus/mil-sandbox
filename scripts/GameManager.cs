using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace MilSandbox.Scripts;

using MilSandbox.Scripts.Tools;
using MilSandbox.Scripts.Autoload;

/// <summary>
/// Main game manager - runs on startup
/// Handles initialization and map loading
/// </summary>
public partial class GameManager : Node
{
	public override void _Ready()
	{
		GD.Print("=== Game Manager Ready ===");
		GD.Print("GameConstants loaded");
		GD.Print($"Grid size: {Autoload.GameConstants.GRID_SIZE_X} x {Autoload.GameConstants.GRID_SIZE_Y}");
		
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

		// Load and instantiate the worldmap scene
		GD.Print("Loading worldmap scene...");
		var worldmapScene = GD.Load<PackedScene>("res://scenes/worldmap.tscn");
		if (worldmapScene != null)
		{
			var worldmap = worldmapScene.Instantiate();
			AddChild(worldmap);
			GD.Print("✓ worldmap scene loaded and instantiated");
		}
		else
		{
			GD.PrintErr("Failed to load worldmap.tscn");
		}
	}
}
