using Godot;
using System;

public partial class Player : Node2D
{
	[Export]
	public float gravity = 1.0f;

	private CharacterBody2D characterBody2D;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		characterBody2D = GetNode<CharacterBody2D>("CharacterBody2D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (characterBody2D.IsOnFloor())
			characterBody2D.Velocity = new Vector2(characterBody2D.Velocity.X, 0);
		else
			characterBody2D.Velocity = new Vector2(characterBody2D.Velocity.X, characterBody2D.Velocity.Y + gravity);
	}
}
