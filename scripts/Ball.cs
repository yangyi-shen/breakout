using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Godot;

public partial class Ball : CharacterBody2D
{
	[Export] public int InitialSpeed { get; set; } = 400;

	public Vector2 ScreenSize;
	public Vector2 BallSize;
	public Vector2 BallInitialPosition;
	public Vector2 BallVelocity;
	public int Speed;
	public Random Random = new Random();

	[Signal] public delegate void hitBrickEventHandler();
	[Signal] public delegate void passedScreenBottomEventHandler();
	[Signal] public delegate void hitScreenTopEventHandler();

	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
		BallSize = GetNode<Sprite2D>("sprite").Texture.GetSize();
		BallInitialPosition = Position;

		NewRound();
	}

	public override async void _Process(double delta)
	{
		// handle collision with other entities
		Vector2 ballSpeed = BallVelocity * (float)delta;
		KinematicCollision2D collision = MoveAndCollide(ballSpeed);
		if (collision != null)
		{
			GodotObject collidedNode = collision.GetCollider();
			if (collidedNode.HasMethod("OnHitByBall")) // if collided node is a brick
			{
				EmitSignal(SignalName.hitBrick); // tell rest of game a brick has been hit
				collidedNode.Call("OnHitByBall"); // tell brick it's been struck

				// increase speed variable and update ball velocity
				Speed += 50;
				BallVelocity = BallVelocity.Normalized() * Speed;
			}

			// move ball in response to collision
			Vector2 collisionNormal = collision.GetNormal();

			// if colliding horizontally
			if (collisionNormal.X == 1 || collisionNormal.X == -1)
			{
				BallVelocity = new Vector2(BallVelocity.X * -1, BallVelocity.Y);
			}

			// if colliding vertically
			if (collisionNormal.Y == 1 || collisionNormal.Y == -1)
			{
				BallVelocity = new Vector2(BallVelocity.X, BallVelocity.Y * -1);
			}
		}

		// handle colliding with screen top
		if (Position.Y < 0)
		{
			BallVelocity = new Vector2(BallVelocity.X, BallVelocity.Y * -1); // bounce
			EmitSignal(SignalName.hitScreenTop); // tell rest of game ball has hit screen top
		}

		// handle passing screen bottom (losing a life)
		float deltaY = BallVelocity.Y * (float)delta;
		if (Position.Y > ScreenSize.Y && Position.Y < ScreenSize.Y + deltaY)
		{
			// extract current lives from lives label
			Label livesLabel = GetNode<Label>("../../HUD/label_lives");
			int currentLives = Regex.Matches(livesLabel.Text, "x").Count;

			if (currentLives == 1) // if game over
			{
				EmitSignal(SignalName.passedScreenBottom);
			}
			else
			{
				EmitSignal(SignalName.passedScreenBottom);
				await ToSignal(GetTree().CreateTimer(1.0), SceneTreeTimer.SignalName.Timeout);
				NewRound(); // start new round after 1 second pause
			}
		}

		// handle colliding with screen sides
		float ballWidth = BallSize.X / 2;
		if (Position.X < ballWidth || Position.X > ScreenSize.X - ballWidth)
		{
			BallVelocity = new Vector2(BallVelocity.X * -1, BallVelocity.Y);
		}
	}

	private void NewRound()
	{
		// place ball above player paddle
		Vector2 playerPosition = GetNode<CharacterBody2D>("../player").GlobalPosition;
		Position = new Vector2(
			x: playerPosition.X,
			y: BallInitialPosition.Y
		);

		// point ball towards random direction & reset speed
		double randomNumber = Random.NextDouble();
		int randomSide = randomNumber < 0.5 ? -1 : 1;
		BallVelocity = new Vector2(randomSide, -1).Normalized() * InitialSpeed;
		Speed = InitialSpeed;
	}
}
