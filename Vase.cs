using Godot;
using System;

public partial class Vase : Node2D
{
	public float gravity = 0.1f;

	private AudioStreamPlayer2D vaseBreakSound;
	private AudioStreamPlayer2D swipeSound;
	private AudioStreamPlayer2D owSound;
	private AnimatedSprite2D sprite;
	private bool swiped = false;
	private bool falling = false;
	private bool breaking = false;
	private float velocityY = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		vaseBreakSound = GetNode<AudioStreamPlayer2D>("VaseBreakSound");
		swipeSound = GetNode<AudioStreamPlayer2D>("SwipeSound");
		owSound = GetParent().GetNode<AudioStreamPlayer2D>("OwSound");
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _PhysicsProcess(double delta)
	{
        if (swiped)
        {
			GlobalPosition += new Vector2(2, 0);
			if (GlobalPosition.X > 3856)
			{
				swiped = false;
				falling = true;
			}
        }
		if (falling)
		{
			velocityY += gravity;
			GlobalPosition += new Vector2(0, velocityY);
		}

		base._PhysicsProcess(delta);
	}

	private void _on_cat_entered(Node2D body)
	{
		if (!swiped)
		{
			swiped = true;
			swipeSound?.Play();
		}
	}

	private void _on_npc_entered(Node2D area)
	{
		if (!breaking)
		{
			breaking = true;
			falling = false;
			vaseBreakSound?.Play();
			velocityY = 0;
			sprite.Play("breaking");
		}
	}

	private void _on_finish_break()
	{
		if (breaking)
		{
			GetParent().RemoveChild(this);
			owSound?.Play();
		}
	}
}
