using Godot;
using System;
using MRalozerca2.Scripts;

public class GamePausedUI : PopupPanel
{
    private void OnGamePaused()
    {
        this.Popup_();
    }

    private void OnGameResumed()
    {
        this.Hide();
    }

    public void TogglePause()
    {
        RectSize = GetViewportRect().Size;
        var manager = this.GetManager();
        var signal = manager.IsGamePaused ? nameof(GameManager.OnGameResumed) : nameof(GameManager.OnGamePaused);
        manager.EmitSignal(signal);
    }

    public override void _Notification(int what)
    {
        base._Notification(what);

        if (what == NotificationWmGoBackRequest)
            TogglePause();
    }

    // Fix for the popup not resizing when the window is resized
    private void OnWindowResized()
    {
        RectSize = GetViewportRect().Size;
    }

    public override void _Ready()
    {
        base._Ready();

        this.GetManager().Connect(nameof(GameManager.OnGamePaused), this, nameof(OnGamePaused));
        this.GetManager().Connect(nameof(GameManager.OnGameResumed), this, nameof(OnGameResumed));
        GetTree().Root.Connect("size_changed", this, nameof(OnWindowResized));
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (Input.IsActionJustPressed("pause"))
            TogglePause();
    }
}
