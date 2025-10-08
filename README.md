# Flocking 2 – Godot 2D Rocket Game

## Project Overview
**Flocking 2** is a 2D Godot game where you control a rocket navigating a field of alien boids. The boids use **Flocking AI** to move together naturally, creating a dynamic and challenging environment.

## How I Implemented It
- The player rocket is a **`PlayerRocket` node** with an `AnimatedSprite2D` for visuals and movement logic in `PlayerRocket.cs`.  
- The boids are **`Boid` nodes** with a `CollisionPolygon2D` as their target, giving them collision detection and flocking behavior.  
- I implemented **`Boid.cs`** to handle the separation, alignment, and cohesion, and targeting rules for the flock.  
- The rocket can die when it collides with boids, triggering an **explosion animation** and stopping the HUD timer.  
- Boid count and speed are adjustable, and the HUD shows elapsed time during gameplay.

## Why Flocking Matters
Using flocking makes the boids move like a real swarm instead of randomly. It adds:
- **Dynamic challenge:** The flock moves together and reacts to the rocket.  
- **Immersive feel:** The movement feels alive and natural.  
- **Replay variety:** Every run is slightly different because the flock adjusts in real time.

## Controls
- Arrow keys (or WASD and space bar) to move the rocket.  
- Avoid collisions with the flock.  
- Survive as long as possible.
