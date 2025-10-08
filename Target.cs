using Godot;
using System;

public partial class Target : RigidBody2D
{
	// Called when node enters scene tree for the first time
	public override void _Ready()
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		//Vector2 v = GetPosition();
		if(GetPosition().Y > 900)
		{
			GravityScale = -20;
		}
		else
		{
			GravityScale=0.05F;
		}
	}
}
