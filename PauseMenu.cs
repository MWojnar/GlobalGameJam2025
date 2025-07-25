using Godot;

public partial class PauseMenu : CanvasLayer
{
    private Button[] buttons;
    private int selectedIndex = 0;

    public override void _Ready()
    {
        // Get all buttons in order
        buttons = new Button[]
        {
            GetNode<Button>("Control/CenterContainer/Panel/VBoxContainer/ContinueButton"),
            GetNode<Button>("Control/CenterContainer/Panel/VBoxContainer/ExitToMenuButton"),
            GetNode<Button>("Control/CenterContainer/Panel/VBoxContainer/ExitGameButton")
        };

        // Set initial selection
        UpdateButtonSelection();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionReleased("ui_pause"))
        {
            UnpauseGame();
            GetViewport().SetInputAsHandled();
            return;
        }

        if (@event.IsActionPressed("ui_up"))
        {
            selectedIndex = (selectedIndex - 1 + buttons.Length) % buttons.Length;
            UpdateButtonSelection();
            GetViewport().SetInputAsHandled();
        }
        else if (@event.IsActionPressed("ui_down"))
        {
            selectedIndex = (selectedIndex + 1) % buttons.Length;
            UpdateButtonSelection();
            GetViewport().SetInputAsHandled();
        }
        else if (@event.IsActionReleased("ui_accept"))
        {
            buttons[selectedIndex].EmitSignal(Button.SignalName.Pressed);
            GetViewport().SetInputAsHandled();
        }
    }

    private void UpdateButtonSelection()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i == selectedIndex)
            {
                buttons[i].GrabFocus();
            }
        }
    }

    private void _on_continue_button_pressed()
    {
        UnpauseGame();
    }

    private void _on_exit_to_menu_button_pressed()
    {
        GetTree().Paused = false;
        GetTree().ChangeSceneToFile("res://TitleScreen.tscn");
    }

    private void _on_exit_game_button_pressed()
    {
        GetTree().Quit();
    }

    private void UnpauseGame()
    {
        GetTree().Paused = false;
        QueueFree();
    }
}