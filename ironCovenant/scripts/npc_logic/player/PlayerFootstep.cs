using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerFootstep : Node3D
{
    //TODO SETUP:

    //TODO This script uses group "sound_[surfacetype]" to define which surfaces make which sound

    //TODO END OF SETUP


    [Export] private CharacterBody3D _player;
    [Export] private RayCast3D _footRaycast;
    [Export] private AudioStreamPlayer3D _footstepPlayer;
    private float _footstepInterval = 0.6f;
    private float _footstepTimer = 0.0f;

    enum SurfaceType
    {
        Tile,
        Concrete,
        Grass,
        Wood
    }
    private Dictionary<SurfaceType, AudioStream[]> _footstepSounds;



    public override void _Ready()
    {
        if (_footstepPlayer == null || _footRaycast == null || _player == null)
        {
            GD.PrintErr($"{Name} not assigned in inspector.");
            _footstepPlayer = GetNode<AudioStreamPlayer3D>("FootstepPlayer");
        }


        _footstepSounds = new Dictionary<SurfaceType, AudioStream[]>
        {
            {
                SurfaceType.Tile,
                new AudioStream[]
                {
                    GD.Load<AudioStream>("res://assets/audio/player/footsteps/tile1.wav"),
                    GD.Load<AudioStream>("res://assets/audio/player/footsteps/tile2.wav"),
                    GD.Load<AudioStream>("res://assets/audio/player/footsteps/tile3.wav")
                }
            },
            {
                SurfaceType.Concrete,
                new AudioStream[]
                {
                    GD.Load<AudioStream>("res://assets/audio/player/footsteps/concrete1.wav"),
                    GD.Load<AudioStream>("res://assets/audio/player/footsteps/concrete2.wav"),
                    GD.Load<AudioStream>("res://assets/audio/player/footsteps/concrete3.wav")
                }
            },
            {
                SurfaceType.Grass,
                new AudioStream[]
                {
                    GD.Load<AudioStream>("res://assets/audio/player/footsteps/grass1.wav"),
                    GD.Load<AudioStream>("res://assets/audio/player/footsteps/grass2.wav"),
                    GD.Load<AudioStream>("res://assets/audio/player/footsteps/grass3.wav")
                }
            },
            {
                SurfaceType.Wood,
                new AudioStream[]
                {
                    GD.Load<AudioStream>("res://assets/audio/player/footsteps/wood1.wav"),
                    GD.Load<AudioStream>("res://assets/audio/player/footsteps/wood2.wav"),
                    GD.Load<AudioStream>("res://assets/audio/player/footsteps/wood3.wav")
                }
            }
        };
    }


    private SurfaceType GetSurfaceSoundType()
    {
        if (_footRaycast.IsColliding())
        {
            var collider = _footRaycast.GetCollider() as Node;

            foreach (SurfaceType surface in Enum.GetValues(typeof(SurfaceType)))
            {
                if (collider != null && collider.IsInGroup("sound_" + surface.ToString().ToLower()))
                {
                    return surface;
                }
            }
        }

        return SurfaceType.Tile;
    }

    private void HandleFootstep(float delta)
    {
        var stepInterval = _footstepInterval;

        var inputDir = Input.GetVector("left", "right", "up", "down");
        var isMoving = inputDir.Length() > 0.1f && _player.IsOnFloor();

        var player = _player as Player;
        if (player.CurrentMoveState == Player.PlayerMoveState.Sprint)
        {
            stepInterval *= 0.5f;
        }
        else if (player.CurrentMoveState == Player.PlayerMoveState.Crouch)
        {
            stepInterval *= 2.0f;
        }

        if (isMoving && _player.IsOnFloor())
        {
            _footstepTimer -= delta;
            if (_footstepTimer <= 0.0f)
            {
                PlayFootstepSound();
                _footstepTimer = stepInterval;
            }
        }
    }

    private void PlayFootstepSound()
    {
        var surface = GetSurfaceSoundType();
        if (_footstepSounds.TryGetValue(surface, out var sounds))
        {
            if (sounds.Length > 0)
            {
                var stream = sounds[GD.Randi() % sounds.Length];
                _footstepPlayer.Stream = stream;
                _footstepPlayer.Play();
            }
        }
    }


    public override void _PhysicsProcess(double delta)
    {
        HandleFootstep((float)delta);
    }

}
