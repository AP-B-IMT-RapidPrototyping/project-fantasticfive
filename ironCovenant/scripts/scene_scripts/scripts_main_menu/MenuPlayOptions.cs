using Godot;
using System;

public partial class MenuPlayOptions : Control
{
    [Export] private AnimationPlayer _animCamSwitch;

    [Export] private PackedScene _gameScene;
    [Export] private PackedScene _newGameScene;

    private void OnContinuePressed()
    {
        if (_gameScene == null)
        {
            GD.PrintErr($"Please assign scenes in the {Name} inspector.");
            return;
        }
        GetTree().ChangeSceneToPacked(_gameScene);
    }

    private void OnNewGamePressed()
    {
        if (_newGameScene == null)
        {
            GD.PrintErr($"Please assign scenes in the {Name} inspector.");
            return;
        }
        GetTree().ChangeSceneToPacked(_newGameScene);
    }

    private void OnBackPressed()
    {
        _animCamSwitch.PlayBackwards("play");
    }
}