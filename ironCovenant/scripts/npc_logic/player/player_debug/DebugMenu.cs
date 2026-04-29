using Godot;
using System;

public partial class DebugMenu : Control
{
    // States
    private bool _debugMenuActive = false;

    // Exports
    [Export] private Pause _pauseMenu;
    [Export] private PlayerHead _playerHead;

    // DebugStats exports
    [Export] private Control _debug;
    [Export] private ColorRect _debugStatsContainer;
    [Export] private SubViewportContainer _camToPlayer;
    [Export] private LineEdit _cin;




    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
    }


    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("escape") && !@event.IsEcho())
        {
            if (GetTree().Paused)
            {
                Visible = false;
            }
        }

        if (@event.IsActionPressed("debug") && !@event.IsEcho() && !_pauseMenu.IsPaused)
        {
            if (!GetTree().Paused)
            {
                Input.MouseMode = Input.MouseModeEnum.Visible;
                GetTree().Paused = true;
                Visible = true;
                _cin.GrabFocus();
            }
            else
            {
                Visible = false;
                GetTree().Paused = false;
                Input.MouseMode = Input.MouseModeEnum.Captured;
            }
        }
    }


    // TOGGLES
    private void ShowDebug()
    {
        _debug.Visible = !_debug.Visible;
    }
    private void ShowDebugStatsContainer()
    {
        _debugStatsContainer.Visible = !_debugStatsContainer.Visible;
    }

    private void ShowCamToPlayer()
    {
        _camToPlayer.Visible = !_camToPlayer.Visible;
    }


    // EVENTS
    private void ShakeExplosion()
    {
        _playerHead.EventShake(2f, 0.7f, 100f, 1.2f); // explosion
    }

    private void ShakeEarthquake()
    {
        _playerHead.EventShake(0.4f, 5.0f, 0.2f, 0.1f); // earthquake
    }

}
