# The Key to Surviving

The Key to Surviving is a 2D cooperative multiplayer fantasy survival game where two players navigate through procedurally varied castle rooms filled with enemies. The objective is simple: survive, find the key, and progress through two increasingly difficult levels.

Each room contains randomized enemy positions and behaviors, ensuring no two runs feel the same. Players must rely on movement, timing, and cooperation to survive.

## Core Gameplay Loop
- Spawn into a room
- Dodge enemies and avoid damage
- Find and collect the key
- Progress to the next room
- Survive both levels to win

*Players lose health when hit by enemies (4 hits = game over).*

## Controls
### Input	Action
- W / A / S / D	Move Player
- Esc	Pause Menu

## Setup Instructions (Unity)
1. Clone the Repository
git clone https://github.com/tpsig/The-Key-to-Surviving.git

*Or download ZIP and extract.*

2. Open in Unity
Open Unity Hub
Click “Add Project”
Select the cloned project folder
Open using the correct Unity version (recommended: Unity 2022 or newer with Netcode for GameObjects installed)
3. Install Dependencies (if needed)

**Make sure these packages are installed via Package Manager:**

- Netcode for GameObjects
- Unity Transport
- TextMeshPro

## How to Test Multiplayer
*This project uses a Host + Client model.*

### Running in Unity Editor
1. Open the scene: **BootstrapScene**
2. Press Play
3. Select NetworkManager
4. Click Host on one instance
5. Open a second instance:
   *Either another Unity Editor window OR A built build (recommended)*
6. Click Client on the second instance

### Recommended Testing Method (Best Practice)
1. Build the game:
- File → Build Settings
- Add all scenes
- Click Build
- Run the Build (Client)
- Run Unity Editor (Host)

## Connection Flow
- One player starts the lobby (Host)
- Second player joins via Client button
- Both players spawn in same room instance
- Shared game state is synchronized via Netcode
## Key Scripts & Architecture
### Core Game Systems
1. GameManager.cs
- Handles game state (win/lose, room progression)
- Singleton pattern
- Tracks key collection and level flow
2. PlayerController.cs
- Movement and input handling
- Damage detection
- Player state logic
3. PlayerSpawner.cs
- Handles spawning players across network sessions
- Uses OnNetworkSpawn

## Networking
- Netcode for GameObjects (NGO)
- Host-client architecture
- Synchronization of:
    - Player positions
    - Health values
    - Game state (rooms, win/lose)

## Gameplay Systems
- Enemy movement system (randomized behavior per room)
- Key pickup / level progression system
- Scene transitions (Start → Room 1 → Room 2 → Game Over)

## Audio & Lighting
1. AudioManager.cs
  - Singleton audio controller for background music + future SFX
2. Global lighting system
  - Orange-tinted light for fantasy atmosphere
  - Enhances readability and mood

## Technical Architecture

**This project demonstrates:**

- Design Patterns Used
- Singleton Pattern
- GameManager
- AudioManager
- DatabaseManager
- Save/Load State
- Event-Driven Architecture
- Player damage events
- Network spawn events (OnNetworkSpawn, OnDestroy)
- Factory Pattern (Enemies)

## Scene Structure
1. Start Scene
   - Automatic Multiplayer lobby creation
   - Start game / View high scores
2. Scene 1: Castle Prison
   - Easier introductory level
3. Scene 2: Castle Dining Hall
   - Harder level with increased difficulty
4. Scene 3: Game Over
5. Restart or exit options

## Data Persistence
- Time completion tracking
- Replayability incentive system

## Known Issues
1. Load and Save State Glitch after multiple runs and exiting through Save and Quit in Pause Menu

## Current workaround:
- Currently disablement of loading a saved state into the DiningRoomScene

## Summary

**This project showcases:**

- Multiplayer networking (Netcode for GameObjects)
- Real-time cooperative gameplay
- Event-driven architecture
- Singleton-based game management
- Basic procedural variation systems
- Clean scene flow and UI structure
