using Godot;
using System;
using System.Threading.Tasks;

public partial class MenuPlayOptions : Control
{
    SceneManager sceneManager;

    [Export] private AnimationPlayer _animCamSwitch;

    [Export] private PackedScene _gameScene;
    [Export] private PackedScene _newGameScene;


    public override void _Ready()
    {
        sceneManager = GetNode<SceneManager>("/root/SceneManager");
    }


    private void OnContinuePressed()
    {
        if (_gameScene == null)
        {
            GD.PrintErr($"Please assign scenes in the {Name} inspector.");
            return;
        }
        GetTree().ChangeSceneToPacked(_gameScene);
    }

    private async void OnNewGamePressed()
    {
        await NewGame();
    }

    private async Task NewGame()
    {
        if (_newGameScene == null)
        {
            GD.PrintErr($"Please assign scenes in the {Name} inspector.");
            return;
        }
        await sceneManager.SwitchScene(_newGameScene.ResourcePath);
    }

    private void OnBackPressed()
    {
        _animCamSwitch.PlayBackwards("play");
    }
}