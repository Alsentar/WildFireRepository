

# Wild Fire – A Roguelike Top-Down Dungeon Crawler (Thesis Project)

A 2D procedural dungeon crawler featuring turn-based combat, dynamic world persistence, and multi-zone generation.

---------------------------------------------------------------------------------------------------------------------

# Project Overview

Wild Fire is a 2D dungeon exploration game developed in Unity as the final thesis project for my Computer Systems Engineering degree.
This indie game combines procedural generation, state persistence, and turn-based combat, resulting in a compact but technically challenging RPG architecture.

The goal of the project was to create a replayable dungeon experience across multiple zones, each with unique visuals, enemy scaling, item placement, and environmental features — while ensuring that the world state remains consistent before and after combat.

---------------------------------------------------------------------------------------------------------------------

# Core Features

Procedural Dungeon Generation (Zone 1 & Zone 2)

Each zone is generated algorithmically at runtime using:

- Randomized room placement
- Corridor generation and smart connections
- Tile-based map composition
- Smart wall rendering (automatic corner/edge tile selection)
- Dynamic object placement (floor tiles, lamps, enemies, stairs, chests)

Zone 2 includes advanced features such as:

- Lamp placement logic
- Tile spacing with coordinate transformation
- Enemy stat scaling per zone
- On-the-fly correction for staircase and chest placement on valid tiles

This allows each playthrough to feel fresh and varied.

---------------------------------------------------------------------------------------------------------------------

# Turn-Based Combat System

Each enemy encounter triggers a dedicated combat scene featuring:

- Player and enemy turn sequencing
- Attack selection (e.g., *Slash*, *Fire*)
- Animated attack types via Animator parameters
- Dynamic stat calculations (attack, defense, speed, HP)
- Feedback through sound effects, UI changes, and animations
- Background changes depending on the zone

Enemies defeated in combat are permanently removed from the overworld.

---------------------------------------------------------------------------------------------------------------------

# Persistent World State (Pre- and Post-Combat)

A key technical challenge of the project was enabling the player to:

1. Enter combat in a separate scene,
2. Return to the overworld,
3. And find the world **exactly as they left it**.

This is accomplished using a custom SavedMapData system that stores:

- Full tile layout (`TileType` matrix → `Zone2TileType` conversion)
- Player position
- Enemy positions + unique enemy IDs
- Eliminated enemy tracking
- Chest positions
- Stair location

The restoration system then reconstructs the dungeon exactly as it was, enabling a seamless gameplay loop between exploration and combat.

---------------------------------------------------------------------------------------------------------------------

# User Interface & UX Systems

The game implements several UI layers:

# Pause Menu

-- Fully functional pause menu
-- Panel toggles for **Party** and **Inventory**
-- Dynamic population of character stats
-- Reusable across zones and scenes

# Player & Enemy Health UI

-- Health bars that animate according to damage
-- Modular Combat UI system
-- Iconography for attacks and actions

# Menu Navigation

-- Main Menu
-- Death screen with responsive scaling
-- Scene transitions

---------------------------------------------------------------------------------------------------------------------

#  Audio System

The game features a centralized AudioManager with:

- Background exploration music
- Footstep SFX tied to movement state
- Attack SFX (Slash, Fire)
- Chest opening SFX
- Zone-specific combat music and backgrounds

This system uses dual AudioSources, SFX layering, and per-zone sound control.

---------------------------------------------------------------------------------------------------------------------

# Art & Visual Style

- Pixel-art assets 
- Dynamic backgrounds for combat that change per zone
- Retro-style lighting (lamps, torches)
- Color-coded environments (Zone 1 vs Zone 2 aesthetics)

---------------------------------------------------------------------------------------------------------------------

# Technical Complexity

This project required integrating multiple advanced Unity systems in a cohesive architecture:

- Multi-scene state management
- Procedural generation with tile classification
- Enum-driven tile systems (`TileType`, `Zone2TileType`)
- Sprite-based world reconstruction
- Runtime instantiation of complex objects (enemies, chests, lamps)
- Combat logic with animation events and SFX triggers
- Bug-resistant placement algorithms (ensuring stairs/chests spawn only on valid floor tiles)
- Persistent enemy tracking using unique IDs

---------------------------------------------------------------------------------------------------------------------

#  Tech Stack

| Component       | Technology                                  |
| --------------- | ------------------------------------------- |
| Engine          | Unity (2021.x+)                             |
| Language        | C#                                          |
| Architecture    | Procedural Generation + Scene-Based Combat  |
| Art Style       | Pixel Art                                   |
| Audio           | Unity AudioSource + Scripted SFX Management |
| Version Control | GitHub                                      |

---------------------------------------------------------------------------------------------------------------------

