using Godot;
using System;

public partial class playerBody : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;

	private bool inAir = true;
	private bool falling = true;
	private bool left = false;
	private bool walking = false;
	private bool moving = false;
	private AnimatedSprite2D animatedSprite;

	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		base._Ready();
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}
		inAir = !IsOnFloor();
		falling = velocity.Y > 0;

		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}
		moving = velocity.X != 0;

		Velocity = velocity;
		MoveAndSlide();
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("ui_left") && !animatedSprite.FlipH)
			animatedSprite.FlipH = true;
		if (Input.IsActionPressed("ui_right") && animatedSprite.FlipH)
			animatedSprite.FlipH = false;
		string desiredAnimation = "idle";
		walking = (Input.IsActionPressed("ui_left") || Input.IsActionPressed("ui_right")) && moving;
		if (inAir)
		{
			if (falling)
				desiredAnimation = "fall";
			else
				desiredAnimation = "jump";
		}
		else if (walking)
			desiredAnimation = "walk";

		if (animatedSprite.Animation != desiredAnimation)
			animatedSprite.Play(desiredAnimation);
		System.Diagnostics.Trace.WriteLine(animatedSprite.Animation);

		base._Process(delta);
	}
}