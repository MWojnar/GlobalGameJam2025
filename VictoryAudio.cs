using Godot;
using System;

public partial class VictoryAudio : AudioStreamPlayer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MainMusic.Instance?.Stop();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!Playing)
		{
			MainMusic.Instance?.Play();
			GetParent().RemoveChild(this);
		}
	}
}
