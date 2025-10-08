using Godot;
using System;

public partial class Flock : Node2D
{
	[Export] public int NumberOfBoids = 5;
	[Export] public PackedScene BoidScene;

	private PlayerRocket _rocket;

	public override void _Ready()
	{
		if (BoidScene == null)
		{
			GD.PrintErr("BoidScene not set.");
			return;
		}

		// search for PlayerRocket starting from the actual scene root
		_rocket = FindRocket(GetTree().CurrentScene);
		if (_rocket == null)
		{
			GD.PrintErr("PlayerRocket not found in the scene!");
			return;
		}

		CallDeferred(nameof(SpawnAllBoids));
	}

	private PlayerRocket FindRocket(Node node)
	{
		if (node is PlayerRocket rocket)
			return rocket;

		foreach (Node child in node.GetChildren())
		{
			var result = FindRocket(child);
			if (result != null)
				return result;
		}

		return null;
	}

	private void SpawnAllBoids()
	{
		var viewport = GetViewportRect().Size;

		for (int i = 0; i < NumberOfBoids; i++)
		{
			Boid boid = BoidScene.Instantiate<Boid>();

			// Pick a random position at least 200px from rocket
			Vector2 spawnPos;
			int attempts = 0;
			do
			{
				spawnPos = new Vector2(GD.Randf() * viewport.X, GD.Randf() * viewport.Y);
				attempts++;
			} while (spawnPos.DistanceTo(_rocket.Position) < 200 && attempts < 100);

			boid.Position = spawnPos;
			AddChild(boid);
			boid.AddToGroup("boids");
		}

		// Delay assigning rocket as target
		GetTree().CreateTimer(0.5f).Timeout += () =>
		{
			foreach (Boid boid in GetTree().GetNodesInGroup("boids"))
				boid.SetTarget(_rocket);
		};
	}
}
