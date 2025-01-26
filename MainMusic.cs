using Godot;
using System;

public partial class MainMusic : AudioStreamPlayer
{
	public static MainMusic Instance { get; private set; }

	public override void _Ready()
	{
		if (Instance == null)
		{
			Instance = this;
			//GetTree().Root.AddChild(this);
		}
		else
			QueueFree();
	}

	public void PlayMusic(AudioStream musicStream = null, bool loop = true)
	{
		var player = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		if (player != null)
		{
			if (musicStream != null)
				player.Stream = musicStream;
			if (!player.Playing)
				player.Play();
		}
	}

	public void StopMusic()
	{
		var player = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		if (player != null)
		{
			player.Stop();
		}
	}
}