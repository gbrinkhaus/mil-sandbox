# Godot Migration Asset Inventory

**Source:** aaa-tilemaps Unity Project  
**Date:** 2025-11-30  
**Purpose:** Assets extracted for Godot 4.x migration

## Directory Structure

### `/scripts` - Game Logic (C#)
All C# scripts from Unity project. Most can be ported to Godot C# with minimal changes.

**Core Systems:**
- `MapController.cs` - Main game controller (needs major refactoring for Godot)
- `MapElement.cs` - Hex tile logic
- `Unit.cs` - Unit behavior and stats
- `Player.cs` - Player data (pure C#, minimal changes needed)
- `gVars.cs` - Global constants and data (convert to Godot resources)
- `UIController.cs` - UI management (needs complete rebuild for Godot Control nodes)
- `CamMove.cs` - Camera control

**UI Controllers:**
- Various button controllers (all need Godot conversion)
- UI element scripts

**Porting Priority:**
1. **Easy**: Player.cs, gVars.cs (data structures)
2. **Medium**: Unit.cs, MapElement.cs (some Unity API calls)
3. **Hard**: MapController.cs, UIController.cs (heavy Unity dependencies)

---

### `/models` - 3D Assets
**Format:** FBX and OBJ - Both import directly into Godot

- `3d_hex.obj` - Hex tile mesh
- `Infantry.fbx` - Infantry unit model
- `Cavalry.fbx` - Cavalry unit model  
- `Artillery.fbx` - Artillery unit model

**Godot Import:** Drag into Godot project, will auto-convert to `.glb`

---

### `/textures` - Image Assets
**Formats:** PNG, JPG, PSD

**Hex Terrain Textures:**
- hex_grass_map.png
- hex_forest_map.png
- hex_desert_map.png
- hex_sea_map.png
- hex_mountain_map.png
- hex_city_map.png
- hex_snow_map.png
- hex_jungle_map.png
- hex_seaice_map.png

**Unit Textures:**
- infantry 1.png
- cav_map.png
- arty2_map.png

**Particle Effects:**
- Various particle textures for explosions/effects

**Water:**
- WaterBasicDaytimeGradient.psd
- WaterBasicNighttimeGradient.psd
- WaterBasicNormals.jpg

**Godot Import:** Direct import, Godot will create `.import` files

---

### `/materials` - Unity Materials (Reference Only)
**Format:** .mat (Unity-specific)

These files document material settings but won't import to Godot.
Use them as reference to recreate materials in Godot's material system.

**Key materials:**
- hex-mat-default.mat
- hex-water-default.mat
- Various unit materials
- Water materials (day/night)

**Godot Conversion:** Create `StandardMaterial3D` resources or shaders

---

### `/sounds` - Audio Files
**Formats:** MP3, WAV

- Cannonshots.mp3
- Classical music track
- Infinity.wav
- Economic Strategy SFX/ folder
- Free Pack/ folder

**Godot Import:** Direct import to AudioStream resources

---

### `/fonts` - Typography
**Format:** TTF

- IMFeENit28P.ttf
- IMFeENrm28P.ttf
- Unipix.ttf
- OpenSans/ folder
- OFL.txt (license file)

**Godot Import:** Direct import as FontFile resources

---

### `/data` - Game Data
- `ori-map.bytes` - Serialized map data (46x92 hex grid)
- `to-dos.txt` - Development tasks
- `ai prompts.txt` - AI assistance notes
- `README.md` - Project overview

**Map Data Format:**  
Pipe-separated values: `fld:X*ctr:Country*cty:City*pwr:X*ind:X*|`

**Godot Conversion:** Parse and convert to JSON or Godot Resource (.tres)

---

## Migration Strategy

### Phase 1: Setup (Week 1)
1. Create new Godot 4.x project (C# enabled)
2. Import 3D models → Test one hex tile rendering
3. Import textures → Apply to test tile
4. Port Player.cs and gVars.cs → Test data structures

### Phase 2: Core Systems (Weeks 2-3)
1. Convert map data format
2. Port MapElement.cs → Create Godot scene/script
3. Implement hex grid generation
4. Port Unit.cs → Create unit scenes

### Phase 3: Game Logic (Weeks 4-5)
1. Refactor MapController.cs for Godot
   - Convert Unity Input → Godot InputEvent
   - Convert Raycasting → Godot PhysicsRayQueryParameters3D
   - Convert GameObject instantiation → PackedScene.Instantiate()
2. Implement movement system
3. Implement combat system

### Phase 4: UI & Polish (Weeks 6-7)
1. Rebuild UI with Godot Control nodes
2. Add fonts and style
3. Add audio
4. Testing and refinement

---

## Files NOT Copied (Intentional)

**Unity-Specific (not useful):**
- .meta files (Unity import settings)
- Standard Assets (Unity's built-in assets)
- TextMesh Pro (Unity package)
- Dark UI (Unity UI package)
- Library/ folder (Unity cache)
- Temp/ folder (Unity temporary files)
- .csproj, .sln files (Unity project files)
- scene1.unity (Unity scene - will be rebuilt in Godot)

**Reason:** These are Unity-proprietary and won't transfer. Godot has its own equivalents.

---

## AI Prompts for Conversion

### For Scripts:
```
"Convert this Unity C# MonoBehaviour script to Godot 4.x Node script: 
[paste script]"
```

### For Materials:
```
"This Unity material uses Standard Shader with these settings: 
[paste .mat contents]. Create equivalent Godot StandardMaterial3D setup."
```

### For Map Data:
```
"Convert this pipe-separated Unity map data to JSON format suitable 
for Godot: [paste sample data]"
```

---

## Next Steps

1. ✅ Assets extracted
2. ⬜ Install Godot 4.x
3. ⬜ Create new Godot project
4. ⬜ Import models/textures
5. ⬜ Create hex tile prototype
6. ⬜ Port Player.cs as proof-of-concept
7. ⬜ Evaluate viability before full migration

---

**Good luck with the migration! 🚀**
