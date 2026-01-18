using Godot;
using System.Collections.Generic;

namespace MilSandbox.Scripts.Autoload;

/// <summary>
/// Global game constants ported from gVars.cs
/// Register as autoload: GameConstants
/// </summary>
public partial class GameConstants : Node
{
    // Grid configuration
    public const int GRID_SIZE_X = 46;
    public const int GRID_SIZE_Y = 92;

    // Hex geometry
    public const float HEXAGON_WIDTH = 1.0f;
    public const float HEXAGON_HEIGHT = 0.866025403784f; // sqrt(3) / 2
    public const float SECTOR_WIDTH = HEXAGON_WIDTH * 0.75f;

    // Map positioning
    public const int MAP_X = -10;
    public const int MAP_Y = 5;
    public const float MAP_Y_LEVEL = 0.51f;

    // Game constants
    public const int NUM_PLAYERS = 4;
    public const int UNIT_SPEED_DIVISOR = 10;
    public const int CAM_SPEED_DIVISOR = 2;
    public const int LIGHT_SPEED_DIVISOR = 10;

    // Income types
    public const int INCOME_TYPE_POWER = 0;
    public const int INCOME_TYPE_INDU = 1;

    // Default color
    public static readonly Color DEFAULT_COLOR = new Color(0.68f, 0.68f, 0.68f, 1.0f);

    // Terrain types
    public static readonly string[] FIELD_NAMES = [
        "grass",    // 0
        "forest",   // 1
        "desert",   // 2
        "sea",      // 3
        "mountain", // 4
        "city",     // 5
        "snow",     // 6
        "jungle",   // 7
        "seaice"    // 8
    ];

    // Countries
    public static readonly string[] COUNTRIES = [
        "Ägypten", "Afghanistan", "Alaska", "Alberta/BC", "Argentinien", "Brasilien",
        "China", "Großbritannien", "Grönland", "Indien", "Indonesien", "Irkutsk",
        "Island", "Jakutien", "Japan", "Kamtschatka", "Kongo", "Madagaskar",
        "Mittelamerika", "Mitteleuropa", "Mittlerer Osten", "Mongolei",
        "Nordwestafrika", "Ontario/Manitoba", "Ost-Australien", "Ostafrika",
        "Oststaaten", "Peru", "Philippinen/Guinea", "Quebec/Neufundland", "Siam",
        "Sibirien", "Skandinavien", "Südafrika", "Südeuropa", "Ukraine", "Ural",
        "Venezuela", "West-Australien", "Westeuropa", "Westrussland", "Weststaaten",
        "Yukon/Nordwest"
    ];

    // City values
    public static readonly Dictionary<string, Dictionary<string, int>> CITY_VALUES = new()
    {
        { "Algiers", new Dictionary<string, int> { { "power", 10 }, { "indu", 1 } } },
        { "Anchorage", new Dictionary<string, int> { { "power", 0 }, { "indu", 0 } } },
        { "Antananarivo", new Dictionary<string, int> { { "power", 0 }, { "indu", 0 } } },
        { "Bangkok", new Dictionary<string, int> { { "power", 12 }, { "indu", 1 } } },
        { "Beijing", new Dictionary<string, int> { { "power", 26 }, { "indu", 3 } } },
        { "Berlin", new Dictionary<string, int> { { "power", 18 }, { "indu", 16 } } },
        { "Bratsk", new Dictionary<string, int> { { "power", 0 }, { "indu", 0 } } },
        { "Buenos Aires", new Dictionary<string, int> { { "power", 10 }, { "indu", 1 } } },
        { "Caracas", new Dictionary<string, int> { { "power", 10 }, { "indu", 1 } } },
        { "Daressalam", new Dictionary<string, int> { { "power", 3 }, { "indu", 0 } } },
        { "Edmonton", new Dictionary<string, int> { { "power", 0 }, { "indu", 1 } } },
        { "Istanbul", new Dictionary<string, int> { { "power", 15 }, { "indu", 1 } } },
        { "Jakarta", new Dictionary<string, int> { { "power", 17 }, { "indu", 2 } } },
        { "Jekaterinburg", new Dictionary<string, int> { { "power", 17 }, { "indu", 3 } } },
        { "Kabul", new Dictionary<string, int> { { "power", 6 }, { "indu", 1 } } },
        { "Kairo", new Dictionary<string, int> { { "power", 10 }, { "indu", 1 } } },
        { "Kalkutta", new Dictionary<string, int> { { "power", 24 }, { "indu", 3 } } },
        { "Kapstadt", new Dictionary<string, int> { { "power", 8 }, { "indu", 1 } } },
        { "Kiew", new Dictionary<string, int> { { "power", 17 }, { "indu", 2 } } },
        { "Kinshasa", new Dictionary<string, int> { { "power", 5 }, { "indu", 0 } } },
        { "Lima", new Dictionary<string, int> { { "power", 9 }, { "indu", 0 } } },
        { "London", new Dictionary<string, int> { { "power", 16 }, { "indu", 21 } } },
        { "Los Angeles", new Dictionary<string, int> { { "power", 15 }, { "indu", 11 } } },
        { "Magadan", new Dictionary<string, int> { { "power", 0 }, { "indu", 0 } } },
        { "Manila", new Dictionary<string, int> { { "power", 9 }, { "indu", 1 } } },
        { "Mexiko-Stadt", new Dictionary<string, int> { { "power", 12 }, { "indu", 1 } } },
        { "Novosibirks", new Dictionary<string, int> { { "power", 5 }, { "indu", 0 } } },
        { "Nuuk", new Dictionary<string, int> { { "power", 0 }, { "indu", 0 } } },
        { "Ottawa", new Dictionary<string, int> { { "power", 0 }, { "indu", 1 } } },
        { "Paris", new Dictionary<string, int> { { "power", 19 }, { "indu", 7 } } },
        { "Perth", new Dictionary<string, int> { { "power", 3 }, { "indu", 1 } } },
        { "Quebec", new Dictionary<string, int> { { "power", 3 }, { "indu", 1 } } },
        { "Reykjavik", new Dictionary<string, int> { { "power", 0 }, { "indu", 0 } } },
        { "Rio de Janeiro", new Dictionary<string, int> { { "power", 13 }, { "indu", 1 } } },
        { "St. Petersburg", new Dictionary<string, int> { { "power", 18 }, { "indu", 3 } } },
        { "Stockholm", new Dictionary<string, int> { { "power", 10 }, { "indu", 3 } } },
        { "Sydney", new Dictionary<string, int> { { "power", 3 }, { "indu", 1 } } },
        { "Tokyo", new Dictionary<string, int> { { "power", 16 }, { "indu", 2 } } },
        { "Ulanbataar", new Dictionary<string, int> { { "power", 0 }, { "indu", 0 } } },
        { "Washington", new Dictionary<string, int> { { "power", 17 }, { "indu", 16 } } },
        { "Wien", new Dictionary<string, int> { { "power", 20 }, { "indu", 9 } } },
        { "Yakutsk", new Dictionary<string, int> { { "power", 0 }, { "indu", 0 } } },
        { "Yellowknife", new Dictionary<string, int> { { "power", 0 }, { "indu", 0 } } }
    };
}
