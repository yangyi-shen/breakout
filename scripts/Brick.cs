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
		Hide(); // make brick invisible while keeping node so audio can play
		GetNode<AudioStreamPlayer2D>("audio_break").Play(); // play brick breaking sound
	}

	// this method is called after the breaking sound finishes playing
	private void OnAudioBreakFinished()
	{
		QueueFree(); // delete brick from scene
	}
}
