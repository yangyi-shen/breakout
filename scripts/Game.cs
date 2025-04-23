using Godot;

public partial class Game : Node2D
{
	[Signal] public delegate void scoreEventHandler();
	[Signal] public delegate void loseLifeEventHandler();

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}

	private void OnBallHitBrick()
	{
		EmitSignal(SignalName.score); // increase score
	}

	private void OnBallPassedScreenBottom()
	{
		EmitSignal(SignalName.loseLife); // lose a life
	}
}
