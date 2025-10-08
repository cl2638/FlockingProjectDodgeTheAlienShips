using Godot;
using System;

public partial class PlayerRocket : CharacterBody2D
{
	[Export] public float Speed = 300f;

	// Connecting explosion scene
	[Export] public PackedScene ExplosionScene;

	private AnimatedSprite2D _sprite;
	private bool _isDead = false;

	public override void _Ready()
	{
		_sprite = GetNode<AnimatedSprite2D>("Sprite2D/AnimatedSprite2D");

		if (_sprite != null)
		{
			_sprite.Animation = "idle";
			_sprite.Play();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_isDead)
		{
			// Skip input and movement when dead
			return;
		}

		// normal movement
		Vector2 inputDir = Vector2.Zero;
		if (Input.IsActionPressed("move_right")) inputDir.X += 1;
		if (Input.IsActionPressed("move_left")) inputDir.X -= 1;
		if (Input.IsActionPressed("move_down")) inputDir.Y += 1;
		if (Input.IsActionPressed("move_up")) inputDir.Y -= 1;

		inputDir = inputDir.Normalized();
		Velocity = inputDir * Speed;
		MoveAndSlide();

		// Animation handling
		if (_sprite != null)
		{
			if (inputDir == Vector2.Zero)
			{
				if (_sprite.Animation != "idle")
				{
					_sprite.Animation = "idle";
					_sprite.Play();
				}
			}
			else
			{
				if (_sprite.Animation != "fired")
				{
					_sprite.Animation = "fired";
					_sprite.Play();
				}
			}
		}

		// Collision check
		foreach (Boid boid in GetTree().GetNodesInGroup("boids"))
		{
			if (boid.GlobalPosition.DistanceTo(GlobalPosition) < 20) // collision radius
			{
				Die();
				break;
			}
		}
	}

	public void Die()
	{
		if (_isDead) return;

		_isDead = true;
		GD.Print("Rocket has been destroyed!");

		// Spawn explosion at rocket position
		if (ExplosionScene != null)
		{
			Node2D explosionInstance = (Node2D)ExplosionScene.Instantiate();
			GetParent().AddChild(explosionInstance);
			explosionInstance.GlobalPosition = GlobalPosition;
		}

		// Stop HUD timer
		var hud = GetTree().GetRoot().GetNode<HUD>("Main/HUD"); // adjust path to your HUD node
		if (hud != null)
			hud.Call("StopTimer");

		// Release boid targets
		foreach (Boid boid in GetTree().GetNodesInGroup("boids"))
		{
			if (boid.Target == this)
				boid.SetTarget(null);
		}

		// Queue rocket for deletion after 0.2 seconds
		GetTree().CreateTimer(0.2f).Timeout += () =>
		{
			QueueFree();
		};
	}
}
