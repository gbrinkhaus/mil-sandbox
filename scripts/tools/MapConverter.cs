using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace MilSandbox.Scripts.Tools;

/// <summary>
/// Converter to transform Unity map data format to Godot JSON
/// Run this once to convert ori-map.bytes to map_data.json
/// </summary>
public partial class MapConverter : Node
{
    public void ConvertMapData()
    {
        var inputPath = "res://godot-migration-assets/data/ori-map.bytes";
        var outputPath = "res://data/map_data.json";

        // Read the original map file
        using var file = FileAccess.Open(inputPath, FileAccess.ModeFlags.Read);
        if (file == null)
        {
            GD.PrintErr($"Could not open file: {inputPath}");
            return;
        }

        var content = file.GetAsText();
        file.Close();

        // Split by pipe separator
        var entries = content.Split("|");
        var tiles = new List<Dictionary<string, object>>();
        var gridX = 0;
        var gridY = 0;

        const int GridSizeX = 46;
        const int GridSizeY = 92;

        foreach (var entry in entries)
        {
            if (string.IsNullOrWhiteSpace(entry))
                continue;

            // Parse each tile entry
            var tileData = ParseTileEntry(entry);

            if (tileData != null)
            {
                tileData["x"] = gridX;
                tileData["y"] = gridY;
                tiles.Add(tileData);

                // Move to next grid position
                gridY += 1;
                if (gridY >= GridSizeY)
                {
                    gridY = 0;
                    gridX += 1;
                }
            }
        }

        // Create output structure
        var mapData = new Dictionary<string, object>
        {
            { "grid_size", new Dictionary<string, int> { { "x", 46 }, { "y", 92 } } },
            { "tiles", tiles }
        };

        // Write JSON file
        var json = JsonSerializer.Serialize(mapData, new JsonSerializerOptions { WriteIndented = true });
        
        using var outputFile = FileAccess.Open(outputPath, FileAccess.ModeFlags.Write);
        if (outputFile == null)
        {
            GD.PrintErr($"Could not create output file: {outputPath}");
            return;
        }

        outputFile.StoreString(json);
        outputFile.Close();

        GD.Print($"✓ Map conversion complete!");
        GD.Print($"  Converted {tiles.Count} tiles to {outputPath}");
    }

    private static Dictionary<string, object> ParseTileEntry(string entry)
    {
        // Parse format: fld:X*ctr:Country*cty:City*pwr:X*ind:X*
        var parts = entry.Split("*");
        var result = new Dictionary<string, object>
        {
            { "type", 0 },
            { "country", "" },
            { "city", "" },
            { "power", 0 },
            { "industry", 0 }
        };

        foreach (var part in parts)
        {
            var kv = part.Split(":");
            if (kv.Length != 2)
                continue;

            var key = kv[0].Trim();
            var value = kv[1].Trim();

            switch (key)
            {
                case "fld":
                    result["type"] = int.Parse(value);
                    break;
                case "ctr":
                    result["country"] = value;
                    break;
                case "cty":
                    result["city"] = value;
                    break;
                case "pwr":
                    result["power"] = int.Parse(value);
                    break;
                case "ind":
                    result["industry"] = int.Parse(value);
                    break;
            }
        }

        return result;
    }
}
