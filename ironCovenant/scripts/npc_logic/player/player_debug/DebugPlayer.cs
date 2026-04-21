using Godot;
using System;

public partial class DebugPlayer : Control
{
    [Export] private Player _player;
    [Export] private Node3D _playerHead;
    [Export] private Camera3D _playerCamera;

    [Export] private Label _fpsLabel;
    [Export] private Label _moveStateLabel;
    [Export] private Label _actionStateLabel;
    [Export] private Label _speedXLabel;
    [Export] private Label _speedZLabel;
    [Export] private Label _speedYLabel;
    [Export] private Label _headOffsetLabel;
    [Export] private Label _fovLabel;




    public override void _Ready()
    {
        if (_player == null || _moveStateLabel == null)
        {
            GD.PrintErr($"{Name} not assigned in inspector.");
        }
    }



    private void UpdateFpsLabel()
    {
        if (_fpsLabel != null)
        {
            var fps = Engine.GetFramesPerSecond();

            switch (fps)
            {
                case >= 120:
                    _fpsLabel.AddThemeColorOverride("font_color", new Color(1, 1, 1, 1)); // white
                    break;
                case >= 60:
                    _fpsLabel.AddThemeColorOverride("font_color", new Color(0, 1, 0)); // green
                    break;
                case >= 30:
                    _fpsLabel.AddThemeColorOverride("font_color", new Color(1, 1, 0)); // yellow
                    break;
                default:
                    _fpsLabel.AddThemeColorOverride("font_color", new Color(1, 0, 0)); // red
                    break;
            }

            _fpsLabel.Text = $"FPS: {fps}";
        }
    }

    private void UpdateMoveStateLabel()
    {
        if (_moveStateLabel != null && _player != null)
        {
            _moveStateLabel.Text = $"State: {_player.CurrentMoveState}";
        }
    }

    private void UpdateActionStateLabel()
    {
        if (_actionStateLabel != null && _player != null)
        {
            _actionStateLabel.Text = $"State: {_player.CurrentActionState}";
        }
    }

    private void UpdateSpeedXLabel()
    {
        if (_speedXLabel != null && _player != null && _player.CurrentMoveState != Player.PlayerMoveState.NoclipDEBUG)
        {
            _speedXLabel.Text = $"Speed X: {Mathf.Abs(_player.Velocity.X):F2}";
        }
        else
        {
            _speedXLabel.Text = $"Speed X: N/A";
        }
    }

    private void UpdateSpeedZLabel()
    {
        if (_speedZLabel != null && _player != null && _player.CurrentMoveState != Player.PlayerMoveState.NoclipDEBUG)
        {
            _speedZLabel.Text = $"Speed Z: {Mathf.Abs(_player.Velocity.Z):F2}";
        }
        else
        {
            _speedZLabel.Text = $"Speed Z: N/A";
        }
    }

    private void UpdateSpeedYLabel()
    {
        if (_speedYLabel != null && _player != null && _player.CurrentMoveState != Player.PlayerMoveState.NoclipDEBUG)
        {
            _speedYLabel.Text = $"Speed Y: {Mathf.Abs(_player.Velocity.Y):F2}";
        }
        else
        {
            _speedYLabel.Text = $"Speed Y: N/A";
        }
    }

    private void UpdateHeadOffsetLabel()
    {
        if (_headOffsetLabel != null && _playerHead != null)
        {
            _headOffsetLabel.Text = $"Cam: X{_playerHead.Rotation.X:F2}, Z{_playerHead.Rotation.Z:F2}";
        }
    }

    private void UpdateFovLabel()
    {
        if (_fovLabel != null && _playerCamera != null)
        {
            _fovLabel.Text = $"Fov: {_playerCamera.Fov:F0}";
        }
    }



    public override void _Process(double delta)
    {
        UpdateFpsLabel();
        UpdateMoveStateLabel();
        UpdateActionStateLabel();
        UpdateSpeedXLabel();
        UpdateSpeedZLabel();
        UpdateSpeedYLabel();
        UpdateHeadOffsetLabel();
        UpdateFovLabel();
    }
}
