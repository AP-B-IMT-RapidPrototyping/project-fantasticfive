using Godot;
using System;
using System.Security.AccessControl;
using System.Threading.Tasks;

public partial class Pause : Control
{
    public bool IsPaused => GetTree().Paused;

    [Export] private AnimationPlayer _pauseAnimation;
    [Export] private AnimationPlayer _showQuitOptionsAnimation;
    [Export] private AnimationPlayer _fadeAnimation;




    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        Visible = false;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey { Pressed: true })
            if (@event.IsActionPressed("escape"))
            {
                if (GetTree().Paused)
                    UnpauseGame();
                else
                    PauseGame();
            }
    }


    private void PauseGame()
    {
        Input.MouseMode = Input.MouseModeEnum.Visible;
        GetTree().Paused = true;
        _pauseAnimation.Play("show");
        _fadeAnimation.Play("half_fade");
        Visible = true;
    }

    private async void UnpauseGame()
    {
        _fadeAnimation.SpeedScale = 4;
        _fadeAnimation.PlayBackwards("half_fade");

        _pauseAnimation.Stop();
        _pauseAnimation.Seek(0.0, true);
        _showQuitOptionsAnimation.Stop();
        _showQuitOptionsAnimation.Seek(0.0, true);
        await _fadeAnimation.ToSignal(_fadeAnimation, AnimationPlayer.SignalName.AnimationFinished);

        Visible = false;
        GetTree().Paused = false;
        Input.MouseMode = Input.MouseModeEnum.Captured;

        _fadeAnimation.SpeedScale = 1;
    }



    private void OnResumePressed()
    {
        if (_pauseAnimation.IsPlaying() || _showQuitOptionsAnimation.IsPlaying())
            return;

        _showQuitOptionsAnimation.Stop();
        _showQuitOptionsAnimation.Seek(0.0, true);
        UnpauseGame();
    }

    private void OnSettingsPressed()
    {
        if (_pauseAnimation.IsPlaying())
            return;

        GD.Print("Settings button pressed.");
        return;
    }

    private void OnQuitPressed()
    {
        if (_pauseAnimation.IsPlaying())
            return;

        _showQuitOptionsAnimation.Play("show");
    }


    private void OnCancelPressed()
    {
        if (_showQuitOptionsAnimation.IsPlaying())
            return;

        _showQuitOptionsAnimation.PlayBackwards("show");
    }

    private async void OnMainMenuPressed()
    {
        if (_showQuitOptionsAnimation.IsPlaying())
            return;

        _fadeAnimation.Play("fade");
        await _fadeAnimation.ToSignal(_fadeAnimation, AnimationPlayer.SignalName.AnimationFinished);
        GetTree().Paused = false;
        GetTree().ChangeSceneToFile(VariableManager.MainMenuScene);
    }

    private void OnDesktopPressed()
    {
        if (_showQuitOptionsAnimation.IsPlaying())
            return;

        GetTree().Paused = false;
        GetTree().Quit();
    }
}
