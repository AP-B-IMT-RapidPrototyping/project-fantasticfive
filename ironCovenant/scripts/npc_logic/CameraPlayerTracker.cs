using Godot;
using System;

public partial class CameraPlayerTracker : Camera3D
{
    private CharacterBody3D _player;
    private Camera3D _playerCamera;

    private Vector3 _target = Vector3.Zero;

    public override void _Ready()
    {
        _player = GetTree().GetFirstNodeInGroup("player") as CharacterBody3D;
        _playerCamera = _player.GetNode("PlayerCamera") as Camera3D;
    }

    public override void _PhysicsProcess(double delta)
    {
        var playerLocation =_player.GlobalPosition;
        playerLocation.Y = 1; // Keep Y at fixed value

        _target = _target.Lerp(playerLocation, 1 * (float)delta);

        LookAt(_target, Vector3.Up);  
    }

}
