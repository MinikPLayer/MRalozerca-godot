using Godot;
using System;

public class CorruptedSaveUnlocker : Panel
{
    [Export] public NodePath HideButtonPath;
    [Export] public NodePath UnlockButtonPath;
    [Export] public NodePath UnlockLabelPath;

    private Button _hideButton;
    private Button _unlockButton;
    private Label _unlockLabel;

    private int _unlockStage = 0;

    private void OnUnlockClicked()
    {
        if (_unlockStage == 0)
        {
            _unlockLabel.Text = "Are you sure?\nThis will delete your save!";
            _unlockButton.Text = "Confirm";
            _unlockStage++;
        }
        else
        {
            LeaderboardsSave.Instance.UnlockSave();
            _unlockStage = 0;
            _unlockButton.Text = "Unlock";
            this.Hide();
        }
    }

    private void OnHideClicked()
    {
        _unlockStage = 0;
        this.Hide();
    }

    public override void _Ready()
    {
        _hideButton = GetNode<Button>(HideButtonPath);
        _unlockButton = GetNode<Button>(UnlockButtonPath);
        _unlockLabel = GetNode<Label>(UnlockLabelPath);

        _unlockButton.Connect("pressed", this, nameof(OnUnlockClicked));
        _hideButton.Connect("pressed", this, nameof(OnHideClicked));

        if (LeaderboardsSave.Instance.SaveLock)
        {
            this.Show();
        }
        else
        {
            this.Hide();
        }
    }

}
