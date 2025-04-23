using Godot;

public partial class Player : CharacterBody2D
{
	[Export]
	public int Speed { get; set; } = 600;
	[Export]
	public int MovementClamp { get; set; } = 10;

	public Vector2 ScreenSize;
	public Vector2 PlayerSize;

	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
		PlayerSize = GetNode<Sprite2D>("sprite").Texture.GetSize();
	}

	public override void _Process(double delta)
	{
		Vector2 velocity = Vector2.Zero;

		if (Input.IsActionPressed("arrows_left"))
		{
			velocity.X -= 1;
		}
		else if (Input.IsActionPressed("arrows_right"))
		{
			velocity.X += 1;
		}

		// turn velocity unit vector into speed w/ correct magnitude
		if (velocity.Length() > 0)
		{
			velocity = velocity.Normalized() * Speed;
		}

		float spriteWidth = PlayerSize.X;
		float movementRange = ScreenSize.X / 2 - spriteWidth / 2 - MovementClamp;
		float screenXCenter = ScreenSize.X / 2;

		Position += velocity * (float)delta;
		Position = new Vector2(
			x: Mathf.Clamp(Position.X, screenXCenter - movementRange, screenXCenter + movementRange),
			y: Position.Y
		);
	}

	private void OnBallHitScreenTop()
	{
		// decrease paddle width
		Sprite2D playerSprite = GetNode<Sprite2D>("sprite");
		CollisionShape2D playerCollisionShape = GetNode<CollisionShape2D>("collision_shape");

		// switch to narrow paddle sprite
		Texture2D narrowSprite = GD.Load<Texture2D>("res://assets/paddle_narrow.png");
		playerSprite.Texture = narrowSprite;

		// make paddle collision shape fit narrow sprite
		RectangleShape2D narrowCollisionShape = new RectangleShape2D();
		narrowCollisionShape.Size = narrowSprite.GetSize();
		playerCollisionShape.Shape = narrowCollisionShape;
	}
}
