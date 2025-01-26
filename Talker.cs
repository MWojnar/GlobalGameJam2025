using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[Tool]
public partial class Talker : Node2D
{
	[Export]
	public SpriteFrames AnimationFrames = null;
	[Export]
	public Vector2 speechBubbleOffset = new Vector2(16, -50);
	[Export]
	public SpeechBubble.BubbleOrigin speechBubbleOrigin = SpeechBubble.BubbleOrigin.BottomRight;
	[Export]
	public string[] dialogue;
	[Export]
	public double pauseBetweenDialogue = 2.0;
	[Export]
	public bool loopDialogue = true;
	[Export]
	public int maxWidth = 240;
	[Export]
	public DisappearMode disappearMode = DisappearMode.Never;
	[Export]
	public Talker respondTo;
	[Export]
	public bool wait = false;

	private AnimatedSprite2D sprite;
	private int dialogueIndex = 0;
	private bool waitingBetweenDialogue = false;
	private SpeechBubble speechBubble;
	private DateTime lastDialogueEndTime;
	private PackedScene speechBubblePrefab;
	private event Action onFinishedTalking;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		speechBubblePrefab = (PackedScene)ResourceLoader.Load("res://prefabs/speech_bubble.tscn");
		if (AnimationFrames != null)
			sprite.SpriteFrames = AnimationFrames;
		if (!wait)
			createSpeechBubble(dialogue.FirstOrDefault());
		if (respondTo != null)
			respondTo.Subscribe(onReact);
	}

	public void Subscribe(Action action)
	{
		onFinishedTalking += action;
	}

	private void onReact()
	{
		createSpeechBubble(dialogue[dialogueIndex]);
	}

	private void createSpeechBubble(string text)
	{
		GetParent().RemoveChild(speechBubble);

		speechBubble = (SpeechBubble)speechBubblePrefab.Instantiate();
		speechBubble.animationSpeed = 15;
		speechBubble.nubAnchorPosition = GlobalPosition + new Vector2(0, -10);
		speechBubble.isLooping = false;
		speechBubble.maxWidth = maxWidth;
		speechBubble.GlobalPosition = GlobalPosition + speechBubbleOffset;
		speechBubble.origin = speechBubbleOrigin;
		speechBubble.text = text;
		speechBubble.textSize = 12;

		CallDeferred("add_sibling", speechBubble);
		speechBubble.Subscribe(onTextFinished);

		sprite.Play();
	}

	private void onTextFinished()
	{
		waitingBetweenDialogue = true;
		sprite.Frame = 0;
		sprite.Stop();
		lastDialogueEndTime = DateTime.Now;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Engine.IsEditorHint())
		{
			if (AnimationFrames != null && sprite?.SpriteFrames != AnimationFrames)
				sprite.SpriteFrames = AnimationFrames;
		}
		else
		{
			if (waitingBetweenDialogue && (DateTime.Now - lastDialogueEndTime).TotalSeconds > pauseBetweenDialogue)
			{
				dialogueIndex++;
				if (dialogueIndex < dialogue.Length)
				{
					if (disappearMode == DisappearMode.EveryMessage)
					{
						GetParent().RemoveChild(speechBubble);
						onFinishedTalking?.Invoke();
					}
					else
						createSpeechBubble(dialogue[dialogueIndex]);
				}
				else
				{
					dialogueIndex = 0;
					if (loopDialogue)
						createSpeechBubble(dialogue[0]);
					else if (disappearMode == DisappearMode.AtEnd || disappearMode == DisappearMode.EveryMessage)
						GetParent().RemoveChild(speechBubble);
					onFinishedTalking?.Invoke();
				}
				waitingBetweenDialogue = false;
			}
		}
	}

	public enum DisappearMode
	{
		Never,
		EveryMessage,
		AtEnd
	}
}
