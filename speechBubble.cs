using Godot;
using System;
using System.Collections.Generic;


public partial class SpeechBubble : Node2D
{
	[Export]
	public int maxWidth = 24;
	[Export]
	public Vector2 nubAnchorPosition = new Vector2(0, 0);
	[Export]
	public BubbleOrigin origin = BubbleOrigin.TopLeft;
	[Export]
	public Texture2D speechTexture;
	[Export]
	public Texture2D speechNubTexture;
	[Export]
	public Font font;
	[Export]
	public string text = "This is test text.  Blah blah blah blah.";
	[Export]
	public int animationSpeed = 15;
	[Export]
	public bool isLooping = true;
	[Export]
	public int textSize = 12;

	private int width = 24;
	private int height = 24;
	private CollisionPolygon2D collisionPolygon;
	private int gridWidth = 8;
	private int gridHeight = 8;
	private int textIndex = 0;
	private double animationProgress = 0;
	private event Action onAnimationEnd;
	private bool animationEnded = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gridWidth = speechTexture.GetWidth() / 3;
		gridHeight = speechTexture.GetHeight() / 3;
		collisionPolygon = GetNode<StaticBody2D>("StaticBody2D").GetNode<CollisionPolygon2D>("CollisionPolygon2D");
		SetText(text);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		int originOffsetX = (int)origin % 3;
		int originOffsetY = (int)origin / 3;
		float topLeftX = -originOffsetX * width / 2;
		float topLeftY = -originOffsetY * height / 2;
		collisionPolygon.Polygon = new Vector2[]
		{
			new Vector2(topLeftX, topLeftY),
			new Vector2(topLeftX + width, topLeftY),
			new Vector2(topLeftX + width, topLeftY + height),
			new Vector2(topLeftX, topLeftY + height)
		};
		animationProgress += delta;
		int prevTextIndex = textIndex;
		if (animationProgress * animationSpeed > text.Length)
		{
			if (textIndex == text.Length)
				animationEnd();
			textIndex = text.Length;
		}
		else
			textIndex = (int)(animationProgress * animationSpeed) + 1;
		if (prevTextIndex != textIndex)
			QueueRedraw();
	}

	public override void _Draw()
	{
		string currentText = text.Substring(0, textIndex);
		var bounds = font.GetMultilineStringSize(currentText, HorizontalAlignment.Left, maxWidth - gridWidth * 2, textSize);
		width = (((int)bounds.X) / gridWidth) * gridWidth + gridWidth * 2;
		height = (((int)bounds.Y) / gridHeight) * gridHeight + gridHeight * 2;
		int originOffsetX = (int)origin % 3;
		int originOffsetY = (int)origin / 3;
		float topLeftX = -originOffsetX * width / 2;
		float topLeftY = -originOffsetY * height / 2;
		for (int x = 0; x < width; x += gridWidth)
		{
			for (int y = 0; y < height; y += gridHeight)
			{
				int innerRectX = x == 0 ? 0 : gridWidth;
				innerRectX = x == width - gridWidth ? gridWidth * 2 : innerRectX;
				int innerRectY = y == 0 ? 0 : gridHeight;
				innerRectY = y == height - gridHeight ? gridHeight * 2 : innerRectY;
				float xPos = topLeftX + x;
				float yPos = topLeftY + y;
				DrawTextureRectRegion(speechTexture,
					new Rect2(new Vector2(xPos, yPos), new Vector2(gridWidth, gridHeight)),
					new Rect2(new Vector2(innerRectX, innerRectY), new Vector2(gridWidth, gridHeight)));
			}
		}
		DrawTextureRect(speechNubTexture, new Rect2(new Vector2(nubAnchorPosition.X - GlobalPosition.X, topLeftY + height), new Vector2(speechNubTexture.GetWidth(), nubAnchorPosition.Y - GlobalPosition.Y - (topLeftY + height))), false);
		DrawMultilineString(font, new Vector2(topLeftX + gridWidth, topLeftY + gridHeight + font.GetHeight(textSize) / 2), currentText, HorizontalAlignment.Left, maxWidth - gridWidth * 2, textSize, modulate: Color.FromHtml("#000000"));
	}

	public void Subscribe(Action action)
	{
		onAnimationEnd += action;
	}

	public void Unsubscribe(Action action)
	{
		onAnimationEnd -= action;
	}

	public void StartAnimation(string text = null)
	{
		if (text == null)
			text = this.text;
		animationProgress = 0;
	}

	public void SetText(string text)
	{
		this.text = text;

		StartAnimation(text);
	}

	private void animationEnd()
	{
		if (!animationEnded)
		{
			onAnimationEnd?.Invoke();
			if (isLooping)
				StartAnimation();
			animationEnded = true;
		}
	}

	public enum BubbleOrigin
	{
		TopLeft = 0,
		TopMiddle = 1,
		TopRight = 2,
		MiddleLeft = 3,
		MiddleMiddle = 4,
		MiddleRight = 5,
		BottomLeft = 6,
		BottomMiddle = 7,
		BottomRight = 8
	}
}
