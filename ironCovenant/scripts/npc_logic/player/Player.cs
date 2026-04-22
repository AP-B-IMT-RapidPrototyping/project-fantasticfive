using Godot;
using Godot.NativeInterop;
using System;
using System.Collections.Generic;

public partial class Player : CharacterBody3D
{
    // Camera Settings
    [Export] private PlayerHead _playerHead;
    [Export] private Camera3D _playerCamera;

    // Movement Settings
    [ExportGroup("Speed Settings")]
    [Export] private float _walkSpeed = 3.0f;
    [Export] private float _crouchSpeed = 1.5f;
    [Export] private float _sprintSpeed = 6.0f;
    [Export] private float _noclipSpeedDEBUG = 12.0f;
    [Export] private float _noclipSprintSpeedDEBUG = 48.0f;
    [Export] private float _jumpVelocity = 5.0f;

    [ExportGroup("Crouch Settings")]
    [Export] private float _crouchHeight = 0.5f;
    [Export] private float _standingHeight = 1.0f;
    [Export] private float _crouchTransitionSpeed = 8.0f;

    private float _coyoteTime = 0.16f;
    private float _coyoteActiveTime = 0.0f;

    private float _acceleration = 8.0f;
    private float _deceleration = 12.0f;
    private float _airAcceleration = 2.5f;
    private float _airDeceleration = 1.5f;
    private float _airControl = 0.85f;

    private float _maxFallSpeed = 190.0f;

    [ExportGroup("Other Settings")]
    [Export] private bool Hello = false; //! i am useless, please delete me


    // State Machine & States
    // Movement States
    public enum PlayerMoveState
    {
        Idle,
        Walk,
        Sprint,
        Crouch,
        Jump,
        Fall,
        NoclipDEBUG
    }
    private PlayerMoveState _currentMoveState = PlayerMoveState.Idle;
    public PlayerMoveState CurrentMoveState => _currentMoveState;

    // Action States
    public enum PlayerActionState
    {
        None,
        Zoom,
    }
    private PlayerActionState _currentActionState = PlayerActionState.None;
    public PlayerActionState CurrentActionState => _currentActionState;

    private bool _alreadyJumped = false;




    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("zoom") && !@event.IsEcho())
        {
            SwitchActionState(CurrentActionState != PlayerActionState.Zoom
                ? PlayerActionState.Zoom
                : PlayerActionState.None);
        }

        if (@event.IsActionPressed("noclip") && @event is InputEventKey noclipKey && noclipKey.Pressed && !noclipKey.Echo) //! TURN OFF WHEN RELEASE BUILD
        {
            SwitchMoveState(_currentMoveState == PlayerMoveState.NoclipDEBUG
                ? PlayerMoveState.Idle
                : PlayerMoveState.NoclipDEBUG);
        }
    }



    // ================================== //
    // MOVEMENT STATE MACHINE
    // ================================== //
    public void SwitchMoveState(PlayerMoveState newMoveState)
    {
        if (_currentMoveState == newMoveState)
            return;

        _currentMoveState = newMoveState;
    }

    private void UpdateMoveState(bool onFloor, Vector2 inputDir)
    {
        if (_currentMoveState == PlayerMoveState.NoclipDEBUG)
            return;

        if (!onFloor)
        {
            SwitchMoveState(Velocity.Y > 0
                ? PlayerMoveState.Jump
                : PlayerMoveState.Fall); // If velocity Y > 0, then Jump, else Fall
            return;
        }

        if (Input.IsActionPressed("crouch")) // Crouch has priority over sprint and jump
        {
            SwitchMoveState(PlayerMoveState.Crouch);
            return;
        }

        if (Input.IsActionPressed("sprint") && inputDir.Length() > 0.01f)
        {
            SwitchMoveState(PlayerMoveState.Sprint);
            return;
        }

        Vector2 hVel = new Vector2(Velocity.X, Velocity.Z);

        if (hVel.Length() > 0.1f)
        {
            SwitchMoveState(PlayerMoveState.Walk);
            return;
        }

        SwitchMoveState(PlayerMoveState.Idle);
    }

    private void ExecuteMoveState(float delta, bool onFloor, Vector2 inputDir)
    {
        switch (_currentMoveState)
        {
            case PlayerMoveState.Idle:
            case PlayerMoveState.Walk:
            case PlayerMoveState.Sprint:
            case PlayerMoveState.Crouch:
                HandleGround(delta, onFloor, inputDir);
                break;

            case PlayerMoveState.Jump:
            case PlayerMoveState.Fall:
                HandleAir(delta, onFloor, inputDir);
                break;

            case PlayerMoveState.NoclipDEBUG:
                HandleNoclip(delta, inputDir);
                break;
        }
    }

    private void HandleGround(float delta, bool onFloor, Vector2 inputDir)
    {
        HandleGravity(delta, onFloor);
        HandleMovement(delta, onFloor, inputDir);
        HandleJump(delta, onFloor);
        MoveAndSlide();
    }

    // HandleGround == HandleAir, am aware. No need for fix, HandleAir can receive updates in future
    private void HandleAir(float delta, bool onFloor, Vector2 inputDir)
    {
        HandleGravity(delta, onFloor);
        HandleMovement(delta, onFloor, inputDir);
        HandleJump(delta, onFloor);
        MoveAndSlide();
    }

    private void HandleNoclip(float delta, Vector2 inputDir) //! TURN OFF WHEN RELEASE BUILD
    {
        float vertical = 0f;

        if (Input.IsActionPressed("jump")) vertical += 1f;
        if (Input.IsActionPressed("crouch")) vertical -= 1f;

        var moveDir = Transform.Basis * new Vector3(inputDir.X, vertical, inputDir.Y);
        float speed = Input.IsActionPressed("sprint")
            ? _noclipSprintSpeedDEBUG
            : _noclipSpeedDEBUG;

        GlobalPosition += moveDir * speed * delta; // Ignore MoveAndSlide to avoid collision
    }

    private float GetTargetSpeed()
    {
        return _currentMoveState switch
        {
            PlayerMoveState.Sprint => _sprintSpeed,
            PlayerMoveState.Crouch => _crouchSpeed,
            _ => _walkSpeed
        };
    }

    private Vector2 SmoothHorizontalVelocity(Vector2 hVel, Vector2 targetVel, float delta, bool onFloor)
    {
        var targetVelLen = targetVel.Length();

        if (onFloor)
        {
            return targetVelLen > 0.01f
                ? hVel.Lerp(targetVel, _acceleration * delta)
                : hVel.Lerp(Vector2.Zero, _deceleration * delta);
        }
        else
        {
            if (targetVel.Length() > 0.01f)
            {
                float alignment = hVel.Normalized().Dot(targetVel.Normalized());
                float speedModifier = Mathf.Remap(alignment, 1.0f, -1.0f, 1.0f, 2.5f);

                float currentSpeed = hVel.Dot(targetVel.Normalized());
                float addSpeed = Mathf.Max(targetVel.Length() - currentSpeed, 0);

                float accel = _airAcceleration * _airControl * speedModifier * delta;
                accel = Mathf.Min(accel, addSpeed);

                Vector2 newVel = hVel + (targetVel.Normalized() * accel);

                return newVel.Lerp(targetVel, _airControl * 1.5f * delta);
            }
            else
            {
                return hVel.Lerp(Vector2.Zero, _airDeceleration * delta);
            }
        }
    }


    private void HandleGravity(float delta, bool onFloor)
    {
        if (!onFloor)
        {
            var newVelocity = Velocity;
            newVelocity.Y = Mathf.Max(newVelocity.Y - VariableManager.Gravity * delta, -_maxFallSpeed);
            Velocity = newVelocity;
        }
    }

    private void HandleMovement(float delta, bool onFloor, Vector2 inputDir)
    {
        var newVelocity = Velocity;

        var moveDir = Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y);

        var targetSpeed = GetTargetSpeed();

        var hVel = new Vector2(newVelocity.X, newVelocity.Z);
        var targetVel = new Vector2(moveDir.X, moveDir.Z) * targetSpeed;

        hVel = SmoothHorizontalVelocity(hVel, targetVel, delta, onFloor);

        newVelocity.X = hVel.X;
        newVelocity.Z = hVel.Y;

        Velocity = newVelocity;
    }

    private void HandleJump(float delta, bool onFloor)
    {
        if (Input.IsActionJustPressed("jump") && _coyoteActiveTime > 0.0f && !_alreadyJumped && _currentMoveState != PlayerMoveState.Crouch)
        {
            var newVelocity = Velocity;

            _alreadyJumped = true;
            _coyoteActiveTime = 0.0f;

            newVelocity.Y = _jumpVelocity;
            Velocity = newVelocity;
        }

        if (onFloor && Velocity.Y <= 0)
        {
            _alreadyJumped = false;
        }

        _coyoteActiveTime = onFloor
            ? _coyoteTime
            : Mathf.Max(_coyoteActiveTime - delta, 0f);
    }
    // ================================== //
    // END MOVEMENT STATE MACHINE
    // ================================== //


    // ================================== //
    // ACTION STATE MACHINE
    // ================================== //
    public void SwitchActionState(PlayerActionState newActionState)
    {
        if (_currentActionState == newActionState)
            return;

        _currentActionState = newActionState;
    }

    private void ExecuteActionState(float delta)
    {
        switch (_currentActionState)
        {
            case PlayerActionState.None:
                break;

            case PlayerActionState.Zoom:
                // PlayerCamera scripts reads the ActionState
                break;
        }
    }
    // ================================== //
    // END ACTION STATE MACHINE
    // ================================== //



    public override void _PhysicsProcess(double delta)
    {
        var inputDir = Input.GetVector("left", "right", "up", "down");
        if (inputDir.Length() > 1f)
            inputDir = inputDir.Normalized();

        bool onFloor = IsOnFloor();

        UpdateMoveState(onFloor, inputDir);
        ExecuteMoveState((float)delta, onFloor, inputDir);
        ExecuteActionState((float)delta);
    }
}
