# STELLAR BLITZ: 2D Space Shooter in Unity

[![Unity 6](https://img.shields.io/badge/Unity-6000.5.10f1%20(LTS)-black?style=flat-square)](https://unity.com/)
[![Render Pipeline](https://img.shields.io/badge/Render%20Pipeline-URP%2017.5-blue?style=flat-square)](https://unity.com/srp/universal-render-pipeline)
[![Input System](https://img.shields.io/badge/Input-Unity%20New%20Input%20System-orange?style=flat-square)](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.20/manual/index.html)
[![Language](https://img.shields.io/badge/C%23-10.0%20%2F%20.NET%20Standard%202.1-purple?style=flat-square)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Course](https://img.shields.io/badge/Course-21CSE353T%20AVMR-green?style=flat-square)](#academic-information)

Stellar Blitz is a wave-based 2D arcade space shooter developed in Unity 6. The architecture is completely self-bootstrapping from code, utilizing in-memory procedural sprite generation on Texture2D buffers, deterministic 2D kinematic trigger physics, multi-layer parallax scrolling starfields, dynamic particle explosion systems, and an adaptive ScreenSpaceOverlay Canvas UI.

---

## Academic Information

- **Project Title**: Stellar Blitz: Procedural 2D Space Shooter with Real-Time Kinematic Simulation in Unity
- **Student Name**: Sricharan Suresh
- **Register Number**: RA2311003040063
- **Class / Batch**: CSE - A (2023-2027)
- **Course**: 21CSE353T - Augmented, Virtual and Mixed Reality
- **Institution**: SRM Institute of Science and Technology
- **Handling Faculty**: Ms. V. Deepa

---

## Gameplay Overview and Controls

The player controls a defensive spacecraft stationed at the bottom row of the screen, intercepting waves of hostile alien ships that descend from the top. Dodge oncoming ships, maintain accurate firing lines, and survive through progressively challenging attack waves.

| Key | Action |
| :--- | :--- |
| Left Arrow / Right Arrow | Move ship horizontally (clamped to screen boundaries) |
| Space (Hold) | Continuous laser fire with cooldown pacing |
| Space (On Screens) | Start game from Main Menu / Retry mission after Game Over |

---

## Screenshots and Visual Output

### 1. Main Title Screen
Main menu interface displaying game title, mission subtitle, controls guide, and parallax starfield.
![Main Menu](screenshots/Main%20Menu.png)

---

### 2. Gameplay and Wave Progression
Player maneuvering across the lower screen boundary and firing at descending enemy craft.
![Descending enemies and shooting](screenshots/Descending%20enemies,%20shooting.png)

---

### 3. Combat Engagement and Particle Explosions
High-speed laser collision with an enemy vessel, showing white damage flash and particle dispersion.
![Combat and Destruction](screenshots/Fired%20-%20destroy.png)

---

### 4. Mission Failed and Score Summary
Game over screen summarizing final score achieved and total waves cleared.
![Mission Failed](screenshots/Mission%20Fail.png)

---

## Key Features and Technical Architecture

1. **Zero-Setup Bootstrapping (GameBootstrapper.cs)**:
   - Single-component initialization.
   - Automatically sets up Orthographic Camera, Canvas Hierarchy, Starfield Parallax Layers, Player Entity, Enemy Spawners, and Game State Singletons.

2. **Procedural Sprite Synthesis (SpriteFactory.cs)**:
   - Programmatically rasterizes pixel art onto runtime Texture2D instances using algorithmic circle, rectangle, and barycentric triangle drawing.
   - Generates player ships, enemy cruisers, energy bolts, and star particles directly in memory without external image dependencies.

3. **Multi-Layer Parallax Starfield (Starfield.cs)**:
   - Generates 80 distinct stars across depth layers with individual velocity scaling and toroidal boundary wrapping.

4. **Kinematic 2D Physics and Trigger Collision**:
   - Uses Kinematic 2D Rigid Bodies with `useFullKinematicContacts = true` and Circle/Box triggers to ensure reliable collision handling without physical jitter or tunneling.

5. **Wave Difficulty Progression (EnemySpawner.cs)**:
   - Mathematical wave scaling controlling enemy count, descent speed, health pools, and spawn delay per wave.

6. **Damage Feedback and Explosion Effects (ExplosionEffect.cs)**:
   - Invulnerability frames with sprite alpha modulation upon damage and radial particle bursts with velocity dispersion and alpha decay.

---

## Class and Directory Structure

```
Assets/SpaceShooter/Scripts/
|-- SpriteFactory.cs      # Procedural rasterizer for ships, bullets, and stars
|-- GameBootstrapper.cs   # Master runtime bootstrapper (Camera, Canvas, Entities)
|-- GameManager.cs        # Game state machine, scoring, waves, and UI routing
|-- PlayerShip.cs         # Player input, movement boundaries, shooting, I-frames
|-- EnemyShip.cs          # Enemy descent logic, health management, damage feedback
|-- EnemySpawner.cs       # Wave scaling math and dynamic object spawning
|-- Projectile.cs         # Laser bolt trajectory and trigger collision routing
|-- Starfield.cs          # Parallax scrolling star simulation
`-- ExplosionEffect.cs    # Particle dispersion and alpha decay VFX
```

---

## How to Run the Project

1. Open the project in Unity 6000.5.10f1 (or any compatible Unity 6 version).
2. Open `Assets/Scenes/SampleScene.unity`.
3. Ensure an empty GameObject in the scene has the `GameBootstrapper` component attached.
4. Press the Play button in the Unity Editor.
5. Press Space to start the game.

---

## Academic Notice

This project was developed by Sricharan Suresh (Register No: RA2311003040063) for the course 21CSE353T: Augmented, Virtual and Mixed Reality at SRM Institute of Science and Technology.
