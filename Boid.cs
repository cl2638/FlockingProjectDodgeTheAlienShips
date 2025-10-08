using Godot;
using System.Collections.Generic;

public partial class Boid : CharacterBody2D
{
	// Adjustable in the Inspector
	[Export] public float MaxSpeed = 500.0f;
	[Export] public float SeparationWeight = 1.0f;
	[Export] public float AlignmentWeight = 1.0f;
	[Export] public float CohesionWeight = 1.0f;
	[Export] public float FollowWeight = 1.0f;
	[Export] public float FollowRadius = 200.0f;
	[Export] public float SeparationDistance = 100.0f;

	private List<Boid> _neighbors = new();
	private Area2D _detectionArea;
	private CharacterBody2D _target;

	// Make target publicly settable for Option 1
	public CharacterBody2D Target
	{
		get => _target;
		set => _target = value;
	}

	public override void _Ready()
	{
		// Get DetectionArea and connect its signals
		_detectionArea = GetNode<Area2D>("DetectionArea");
		_detectionArea.BodyEntered += _on_DetectionArea_body_entered;
		_detectionArea.BodyExited += _on_DetectionArea_body_exited;

		// Add to boids group so rocket can notify on Die()
		AddToGroup("boids");

		// Randomize starting position and velocity
		var viewportRect = GetViewportRect();
		var randomX = GD.Randf() * viewportRect.Size.X;
		var randomY = GD.Randf() * viewportRect.Size.Y;
		Position = new Vector2(randomX, randomY);

		var randomAngle = GD.Randf() * Mathf.Pi * 2;
		var randomVelocity = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)) * MaxSpeed;
		Velocity = randomVelocity;
		MoveAndSlide();

		// Face movement direction
		LookAt(Position + Velocity);
	}

	public void SetTarget(CharacterBody2D target)
	{
		_target = target;
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 separationVector = Separation() * SeparationWeight;
		Vector2 alignmentVector = Alignment() * AlignmentWeight;
		Vector2 cohesionVector = Cohesion() * CohesionWeight;
		Vector2 followVector = Centralization() * FollowWeight;

		Vector2 direction = (separationVector + alignmentVector + cohesionVector + followVector).Normalized();
		Velocity = Velocity.Lerp(direction * MaxSpeed, (float)delta);
		MoveAndSlide();

		LookAt(Position + Velocity);

		// Debug --  show neighbor count
		//GD.Print($"{Name} sees {_neighbors.Count} neighbors.");
	}

	// Signals for neighbor detection
	private void _on_DetectionArea_body_entered(Node body)
	{
		if (body is Boid otherBoid && otherBoid != this)
			_neighbors.Add(otherBoid);

		// Only kill rocket if this boid has a target
		if (body is PlayerRocket rocket && _target == rocket)
			rocket.Die();
	}

	private void _on_DetectionArea_body_exited(Node body)
	{
		if (body is Boid otherBoid)
			_neighbors.Remove(otherBoid);
	}

	// Separation
	private Vector2 Separation()
	{
		if (_neighbors.Count == 0) return Vector2.Zero;
		Vector2 steer = Vector2.Zero;
		foreach (var neighbor in _neighbors)
		{
			Vector2 diff = Position - neighbor.Position;
			if (diff.Length() < SeparationDistance)
				steer += diff.Normalized();
		}
		return steer.Normalized();
	}

	// Alignment
	private Vector2 Alignment()
	{
		if (_neighbors.Count == 0) return Vector2.Zero;
		Vector2 avgVel = Vector2.Zero;
		foreach (var neighbor in _neighbors)
			avgVel += neighbor.Velocity;
		avgVel /= _neighbors.Count;
		return avgVel.Normalized();
	}

	// Cohesion
	private Vector2 Cohesion()
	{
		if (_neighbors.Count == 0) return Vector2.Zero;
		Vector2 center = Vector2.Zero;
		foreach (var neighbor in _neighbors)
			center += neighbor.Position;
		center /= _neighbors.Count;
		return (center - Position).Normalized();
	}

	// Follow/Targeting
	private Vector2 Centralization()
	{
		if (_target != null)
		{
			if (!IsInstanceValid(_target))
			{
				_target = null;
				return Vector2.Zero;
			}

			if (Position.DistanceTo(_target.Position) < FollowRadius)
				return Vector2.Zero;

			return (_target.Position - Position).Normalized();
		}
		return Vector2.Zero;
	}
}
