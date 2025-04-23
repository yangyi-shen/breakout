using Godot;
using System;

public partial class Brick : StaticBody2D
{
	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}

    // this method is called by the ball when it hits a brick
	private void OnHitByBall()
	{
		QueueFree(); // delete brick from scene
	}
}
