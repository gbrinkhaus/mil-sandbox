# Worldmap Loader Migration Plan

## Overview
Migrate the Unity-based hex map loading system to Godot 4.5, maintaining the 46x92 grid with world-wrapping and all strategic game features.

---

## Phase 1: Data Layer (Week 1)

### 1.1 Convert Map Data Format
**Input:** `ori-map.bytes` (pipe-separated text: `fld:X*ctr:Country*cty:City*pwr:X*ind:X*|`)
**Output:** Godot Resource file

**Tasks:**
- Create `MapData.gd` resource class to hold map configuration
- Write Python/GDScript converter script to parse `.bytes` → JSON
- Create `map_data.json` with structured format:
```json
{
  "grid_size": {"x": 46, "y": 92},
  "tiles": [
    {"x": 0, "y": 0, "type": 8, "country": "", "city": "", "power": 0, "industry": 0},
    ...
  ]
}
```

### 1.2 Port Global Constants (`gVars.cs`)
**Create:** `game_constants.gd` (autoload singleton)

```gdscript
extends Node

# Grid configuration
const GRID_SIZE_X = 46
const GRID_SIZE_Y = 92
const HEXAGON_WIDTH = 0.60
const HEXAGON_HEIGHT = 0.52
const SECTOR_WIDTH = HEXAGON_WIDTH * 0.75
const MAP_X = -10
const MAP_Y = 5
const MAP_Y_LEVEL = 0.51

# Terrain types
const FIELD_NAMES = ["grass", "forest", "desert", "sea", "mountain", "city", "snow", "jungle", "seaice"]

# City data
const CITY_VALUES = {
    "Berlin": {"power": 18, "indu": 16},
    "London": {"power": 16, "indu": 21},
    # ... all cities
}
```

---

## Phase 2: Core Map Classes (Week 2)

### 2.1 Port `MapElement.cs` → `HexTile.gd`
**Create:** `scripts/hex_tile.gd` (extends Node3D)

**Key conversions:**
- `GameObject ftile` → `MeshInstance3D mesh_instance`
- `Renderer frenderer` → `mesh_instance.material_override`
- `Resources.Load()` → `load()` or `preload()`
- `Instantiate()` → `instantiate()`

```gdscript
extends Node3D
class_name HexTile

var country: String
var city: String
var tile_type: String
var grid_x: int
var grid_y: int
var index: int
var power: int = 0
var industry: int = 0
var owner: Player = null
var unit: Unit = null
var move_cost: float = 1.0
var elevation: float = 0.0
var marked: bool = false

var mesh_instance: MeshInstance3D
var material: StandardMaterial3D
```

**Methods to port:**
- `Init()` → `initialize()` - Position calculation, mesh setup, material assignment
- `GetPosition()` → `get_tile_position()`
- `HasUnit()`, `HasFriendlyUnit()`, `IsLandField()`, `IsCity()`
- `GetAngleTo()` for unit rotation

---

### 2.2 Create `MapLoader.gd`
**Responsibility:** Load map data and spawn hex tiles

```gdscript
extends Node
class_name MapLoader

var map_fields: Array[Array] = []  # 2D array of HexTile
var hex_tile_scene: PackedScene = preload("res://scenes/hex_tile.tscn")
var map_initialized: bool = false

func load_map() -> void:
    var map_data = load_map_data()
    _generate_grid(map_data)

func load_map_data() -> Dictionary:
    var file = FileAccess.open("res://data/map_data.json", FileAccess.READ)
    var json = JSON.new()
    var parse_result = json.parse(file.get_as_text())
    file.close()
    return json.data

func _generate_grid(data: Dictionary) -> void:
    map_fields.resize(GameConstants.GRID_SIZE_X)
    
    for i in GameConstants.GRID_SIZE_X:
        map_fields[i] = []
        map_fields[i].resize(GameConstants.GRID_SIZE_Y)
        
        for j in GameConstants.GRID_SIZE_Y:
            var tile_data = data.tiles[i * GameConstants.GRID_SIZE_Y + j]
            var hex_tile = _create_hex_tile(i, j, tile_data)
            map_fields[i][j] = hex_tile
            add_child(hex_tile)
    
    map_initialized = true
```

---

## Phase 3: Map Controller (Week 3)

### 3.1 Port `MapController.cs` → Split into focused classes

**3.1.1 `MapManager.gd`** - Grid access & queries
```gdscript
extends Node
class_name MapManager

var map_loader: MapLoader
var map_fields: Array[Array]  # Reference to MapLoader's grid

func get_tile(x: int, y: int) -> HexTile
func has_friendly_unit(x: int, y: int, player: Player) -> bool
func get_neighbors(tile: HexTile) -> Array[HexTile]
func find_tile_by_object(obj: Node3D) -> HexTile
func unmark_tiles() -> void
```

**3.1.2 `HexCoordinates.gd`** - Hex math utilities
```gdscript
extends Node
class_name HexCoordinates

static func calculate_position(grid_x: int, grid_y: int) -> Vector3:
    var x_pos = (GameConstants.HEXAGON_WIDTH * grid_x * 1.5 + 
                 GameConstants.SECTOR_WIDTH * (grid_y % 2)) + GameConstants.MAP_X
    var y_pos = -(GameConstants.HEXAGON_HEIGHT * grid_y / 2.0) + GameConstants.MAP_Y
    return Vector3(x_pos, GameConstants.MAP_Y_LEVEL, y_pos)

static func get_neighbor_offsets(grid_y: int) -> Array:
    if grid_y % 2 == 0:
        return [[0, -2], [0, -1], [0, 1], [0, 2], [-1, 1], [-1, -1]]
    else:
        return [[0, -2], [1, -1], [1, 1], [0, 2], [0, 1], [0, -1]]

static func wrap_x_coordinate(x: int) -> int:
    # Handle world-edge wrapping
    if x < 0:
        return GameConstants.GRID_SIZE_X - 1
    elif x >= GameConstants.GRID_SIZE_X:
        return 0
    return x
```

---

## Phase 4: Material & Texture System (Week 2-3)

### 4.1 Create Material Manager
**Challenge:** Unity uses `Resources.Load()` to swap textures dynamically
**Solution:** Pre-create materials for each terrain type

**Create:** `scripts/material_manager.gd`
```gdscript
extends Node

var materials: Dictionary = {}

func _ready():
    _load_materials()

func _load_materials():
    for terrain in GameConstants.FIELD_NAMES:
        var mat = StandardMaterial3D.new()
        var texture_path = "res://godot-migration-assets/textures/hex_%s_map.png" % terrain
        mat.albedo_texture = load(texture_path)
        mat.texture_filter = BaseMaterial3D.TEXTURE_FILTER_NEAREST
        materials[terrain] = mat

func get_material(terrain_type: String) -> StandardMaterial3D:
    return materials.get(terrain_type)
```

**Update `HexTile.initialize()`:**
```gdscript
func initialize(grid_x: int, grid_y: int, tile_type: int, country: String, city: String):
    # ... position calculation ...
    
    var terrain_name = GameConstants.FIELD_NAMES[tile_type]
    var material = MaterialManager.get_material(terrain_name)
    mesh_instance.material_override = material
```

---

## Phase 5: Integration (Week 4)

### 5.1 Main Scene Setup
**Update:** `scenes/main.tscn`

```
Main (Node3D)
├── Camera3D
├── DirectionalLight3D
├── MapLoader
└── MapManager
```

**Create:** `scripts/game.gd`
```gdscript
extends Node3D

@onready var map_loader = $MapLoader
@onready var map_manager = $MapManager

func _ready():
    map_loader.load_map()
    map_manager.map_fields = map_loader.map_fields
    map_manager.map_loader = map_loader
```

---

## Phase 6: Testing & Validation (Week 4)

### 6.1 Create Test Scene
**Purpose:** Verify map loads correctly

**Create:** `tests/test_map_load.gd`
```gdscript
extends Node

func _ready():
    var loader = MapLoader.new()
    add_child(loader)
    loader.load_map()
    
    # Verify grid dimensions
    assert(loader.map_fields.size() == 46)
    assert(loader.map_fields[0].size() == 92)
    
    # Verify specific tiles (known from ori-map.bytes)
    var tile_0_0 = loader.map_fields[0][0]
    assert(tile_0_0.tile_type == "seaice")  # fld:8
    
    var groenland_tile = loader.map_fields[0][6]
    assert(groenland_tile.country == "Grönland")
    
    print("✓ Map load test passed")
```

### 6.2 Visual Verification
- Load map in editor
- Verify 46x92 grid appears
- Check world wrapping (tiles at edges)
- Verify city tiles have correct power/industry values

---

## File Structure After Migration

```
mil-sandbox/
├── scenes/
│   ├── hex_tile.tscn           (single hex tile prefab)
│   ├── main.tscn               (game scene)
│   └── tests/
│       └── test_map_load.tscn
├── scripts/
│   ├── autoload/
│   │   ├── game_constants.gd   (autoload)
│   │   └── material_manager.gd (autoload)
│   ├── hex_tile.gd
│   ├── map_loader.gd
│   ├── map_manager.gd
│   ├── hex_coordinates.gd
│   └── game.gd
├── data/
│   └── map_data.json           (converted from ori-map.bytes)
└── materials/
    └── hex_grass_material.tres (already created)
```

---

## Key Migration Challenges & Solutions

| Unity Pattern | Godot Equivalent | Notes |
|--------------|------------------|-------|
| `GameObject.AddComponent<T>()` | `Node.add_child()` | Components → Node hierarchy |
| `Resources.Load()` | `load()` / `preload()` | Same concept, simpler syntax |
| `Instantiate(prefab, pos, rot)` | `scene.instantiate()` + `set_position()` | Two steps in Godot |
| `Renderer.material` | `MeshInstance3D.material_override` | Direct property access |
| `TextAsset.text.Split()` | `FileAccess.open().get_as_text()` | Built-in file I/O |
| 2D Array `[,]` | `Array[Array]` | Godot uses nested arrays |

---

## Success Criteria

- ✅ **Phase 1:** Map data successfully converted to JSON  
- ✅ **Phase 2:** Single hex tile spawns at correct position with correct texture  
- ✅ **Phase 3:** Full 46x92 grid loads without errors  
- ✅ **Phase 4:** All terrain types display correct textures  
- ✅ **Phase 5:** World wrapping works (edge tiles connect)  
- ✅ **Phase 6:** Performance acceptable (60 FPS with 4232 tiles)

---

## Estimated Timeline
**3-4 weeks** for complete worldmap loader migration

## Current Status
**Not Started** - Ready to begin Phase 1
