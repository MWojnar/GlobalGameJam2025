using Godot;


public partial class speechBubble : Node2D
{
	[Export]
	public int width = 24;
	[Export]
	public int height = 24;
	[Export]
	public Texture2D speechTexture;

	private CollisionPolygon2D collisionPolygon;
	private int gridWidth = 8;
	private int gridHeight = 8;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gridWidth = speechTexture.GetWidth() / 3;
		gridHeight = speechTexture.GetHeight() / 3;
		collisionPolygon = GetNode<StaticBody2D>("StaticBody2D").GetNode<CollisionPolygon2D>("CollisionPolygon2D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		collisionPolygon.Polygon = new Vector2[]
		{
			new Vector2(GlobalPosition.X - width / 2, GlobalPosition.Y - height / 2),
			new Vector2(GlobalPosition.X - width / 2, GlobalPosition.Y + height / 2),
			new Vector2(GlobalPosition.X + width / 2, GlobalPosition.Y + height / 2),
			new Vector2(GlobalPosition.X + width / 2, GlobalPosition.Y - height / 2)
		};
	}

	public override void _Draw()
	{
		for (int x = 0; x < width; x += gridWidth)
		{
			for (int y = 0; y < height; y += gridHeight)
			{
				int innerRectX = x == 0 ? 0 : gridWidth;
				innerRectX = x == width - gridWidth ? gridWidth * 2 : innerRectX;
				int innerRectY = y == 0 ? 0 : gridHeight;
				innerRectY = y == height - gridHeight ? gridHeight * 2 : innerRectY;
				float xPos = GlobalPosition.X - width / 2 + x;
				float yPos = GlobalPosition.Y - height / 2 + y;
				DrawTextureRectRegion(speechTexture,
					new Rect2(new Vector2(xPos, yPos), new Vector2(gridWidth, gridHeight)),
					new Rect2(new Vector2(innerRectX, innerRectY), new Vector2(gridWidth, gridHeight)));
			}
		}
	}
}
