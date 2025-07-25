using Godot;

public partial class PauseManager : Node
{
    private PackedScene pauseMenuScene;
    private CanvasLayer pauseMenuInstance;

    public override void _Ready()
    {
        // Load the pause menu scene
        pauseMenuScene = GD.Load<PackedScene>("res://PauseMenu.tscn");
        
        // Set process mode to always so it works even when paused
        ProcessMode = Node.ProcessModeEnum.Always;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionReleased("ui_pause"))
        {
            TogglePause();
            GetViewport().SetInputAsHandled();
        }
    }

    private void TogglePause()
    {
        if (GetTree().Paused)
        {
            // Unpause
            GetTree().Paused = false;
            if (pauseMenuInstance != null)
            {
                pauseMenuInstance.QueueFree();
                pauseMenuInstance = null;
            }
        }
        else
        {
            // Pause
            GetTree().Paused = true;
            
            // Create and add pause menu
            if (pauseMenuScene != null)
            {
                pauseMenuInstance = pauseMenuScene.Instantiate<CanvasLayer>();
                GetTree().CurrentScene.AddChild(pauseMenuInstance);
            }
            else
            {
                GD.PrintErr("PauseMenu scene not loaded!");
            }
        }
    }
}