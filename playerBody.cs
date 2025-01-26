using Godot;
using Godot.Collections;
using System;

public partial class PlayerBody : CharacterBody2D
{
	public const float Speed = 125.0f;
	public const float JumpVelocity = -225.0f;
	public const float HeldJumpTimeLimit = 0.26f;

	private bool inAir = true;
	private bool prevInAir = true;
	private bool falling = true;
	private bool left = false;
	private bool walking = false;
	private bool moving = false;
	private AnimatedSprite2D animatedSprite;
	private DateTime lastJump;
	private AudioStreamPlayer jumpingSound;
	private AudioStreamPlayer landingSound;

	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		jumpingSound = GetNode<AudioStreamPlayer>("JumpingSound");
		landingSound = GetNode<AudioStreamPlayer>("LandingSound");

		base._Ready();
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		if (Input.IsActionPressed("ui_accept") && (IsOnFloor() || (DateTime.Now - lastJump).TotalSeconds < HeldJumpTimeLimit))
		{
			if (IsOnFloor())
			{
				lastJump = DateTime.Now;
				jumpingSound?.Play();
			}
			velocity.Y = JumpVelocity;
		}
		inAir = !IsOnFloor();
		falling = velocity.Y > 0;

		Vector2 direction = new Vector2((Input.IsActionPressed("ui_left") ? -1 : 0) + (Input.IsActionPressed("ui_right") ? 1 : 0), 0);
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
		else
		{
			if (walking)
				desiredAnimation = "walk";
			if (prevInAir)
				landingSound?.Play();
		}

		if (animatedSprite.Animation != desiredAnimation)
			animatedSprite.Play(desiredAnimation);
		prevInAir = inAir;

		base._Process(delta);
	}
}