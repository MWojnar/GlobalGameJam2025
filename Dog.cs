using Godot;
using System;
using static System.Net.Mime.MediaTypeNames;

public partial class Dog : CharacterBody2D
{
	[Export]
	public Node2D cat;
	public float speed = 1.0f;

	private AnimatedSprite2D sprite;
	private bool triggered = false;
	private bool left = true;
	private bool running = false;
	private bool walling = false;
	private SpeechBubble bubble;
	private PackedScene speechBubblePrefab;
	private AudioStreamPlayer2D currentWoof;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		sprite.Stop();
		speechBubblePrefab = (PackedScene)ResourceLoader.Load("res://prefabs/speech_bubble.tscn");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		float catPosX = cat.GetNode<CharacterBody2D>("CharacterBody2D").GlobalPosition.X;
		walling = false;
		if (triggered && cat != null)
		{
			if (catPosX < GlobalPosition.X)
			{
				var collision = MoveAndCollide(new Vector2(-Math.Min(GlobalPosition.X - catPosX, speed), 0));
				if (collision != null)
				{
					Node2D collidingNode = collision.GetCollider() as Node2D;
					if (!(collidingNode is Player))
						walling = true;
				}
				left = true;
				running = true;
			}
			else if (catPosX > GlobalPosition.X)
			{
				var collision = MoveAndCollide(new Vector2(Math.Min(catPosX - GlobalPosition.X, speed), 0));
				if (collision != null)
				{
					Node2D collidingNode = collision.GetCollider() as Node2D;
					if (!(collidingNode is Player))
						walling = true;
				}
				left = false;
				running = true;
			}
			else
				running = false;
			if (currentWoof == null || !currentWoof.Playing)
			{
				currentWoof = getRandomWoof();
				currentWoof.Play();
			}
		}
		sprite.FlipH = !left;
		string desiredAnimation = "default";
		if (walling)
			desiredAnimation = "wall";
		else if (running)
			desiredAnimation = "running";
		if (sprite.Animation != desiredAnimation)
			sprite.Play(desiredAnimation);
		if (bubble != null)
		{
			bubble.nubAnchorPosition = GlobalPosition + new Vector2(0, -10);
			bubble.GlobalPosition = GlobalPosition + new Vector2(0, -50);
		}
	}

	private void _trigger(Node2D body)
	{
		if (!triggered)
		{
			bubble = (SpeechBubble)speechBubblePrefab.Instantiate();
			bubble.animationSpeed = 15;
			bubble.nubAnchorPosition = GlobalPosition + new Vector2(0, -10);
			bubble.isLooping = false;
			bubble.maxWidth = 480;
			bubble.GlobalPosition = GlobalPosition + new Vector2(0, -50);
			bubble.origin = SpeechBubble.BubbleOrigin.BottomMiddle;
			bubble.text = "WOOF!";
			bubble.textSize = 12;

			CallDeferred("add_sibling", bubble);
		}
		triggered = true;
	}

	private AudioStreamPlayer2D getRandomWoof()
	{
		Random random = new Random();
		int randInt = random.Next(5) + 1;
		return GetNode<AudioStreamPlayer2D>("WoofSound" + randInt);
	}
}