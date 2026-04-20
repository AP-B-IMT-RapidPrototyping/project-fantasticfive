using Godot;
using System;
using System.ComponentModel;

public partial class PlayerHead : Node3D
{
    //TODO SETUP:
    //TODO This script is fully independent
    //TODO To add extra effects, write new calculation functions under "CUSTOM CALCULATIONS"
    //TODO Results need to be predefined as variables and calculated in the new function
    //TODO And add the results to the "Apply" function

    //TODO Call function "EventShake" to call shake on command, for example on explosions, earthquakes, etc. 
    //TODO EXAMPLE EventShake USAGE:
    // earthquake ease in and out:
    // EventShake(0.4f, 5.0f, 0.2f, 0.1f); // earthquake

    // explosion, ease out:
    // EventShake(2f, 0.7f, 100f, 1.2f); // explosion

    //TODO END OF SETUP

    [Export] private Player _player;
    [Export] private Camera3D _playerCamera;


    // Mouse Settings
    [ExportGroup("Mouse Settings")]
    [Export] private float _mouseSensitivityX = 0.003f;
    [Export] private float _mouseSensitivityY = 0.003f;
    [Export] private float _mouseSensitivityXZoom = 0.5f;
    [Export] private float _mouseSensitivityYZoom = 0.5f;

    [Export] private float _cameraSmooth = 25.0f;


    // Rotation
    private float _yaw;
    private float _pitch;
    private float _smoothYaw;
    private float _smoothPitch;

    private float _maxPitch = Mathf.DegToRad(89f);
    private float _minPitch = Mathf.DegToRad(-89f);

    private bool _cameraLocked = false;


    // Camera FOV Settings
    [ExportGroup("FOV Settings")]
    [Export] private float _fovDefault = 90.0f;
    [Export] private float _fovSprint = 100.0f;
    [Export] private float _fovCrouch = 80.0f;
    [Export] private float _fovJump = 95.0f;
    [Export] private float _fovFall = 120.0f;
    [Export] private float _fovZoom = 40.0f;

    [Export] private float _fovSmooth = 8.0f;
    [Export] private float _fovJumpSmooth = 5.0f;
    [Export] private float _fovFallSmooth = 0.2f;

    private bool _fovLocked = false;


    // Head Tilt Settings
    [ExportGroup("Head Tilt Settings")]
    [Export] private float _tiltMoveAmount = -0.03f;
    [Export] private float _tiltMoveAmountStrafe = 0.03f;
    [Export] private float _tiltFallAmount = 0.008f;
    [Export] private float _tiltJumpMax = 0.10f; // jump tilt   
    [Export] private float _tiltFallMax = 0.90f; // falling tilt
    [Export] private float _tiltSmooth = 10f;

    [ExportGroup("Fall Sway")]
    [Export] private bool _fallSwayEnabled = true;
    [Export] private float _fallSwaySpeed = 1f;
    [Export] private float _fallSwayAmount = 0.3f;

    [ExportGroup("Fall Thresholds")]
    [Export] private float _fallBigThreshold = 10f;
    [Export] private float _fallSmallThreshold = 4f;
    [Export] private float _landImpact = 1.25f;

    private float _tiltX;
    private float _tiltZ;
    private float _landingTiltX;
    private float _landingTiltZ;

    private float _tiltFallSide = 0f;
    private float _fallPeakSpeed;
    private bool _fallSideDecided = false;
    private bool _wasOnFloor = true;


    // Head Bob settings
    [ExportGroup("Head Bob Settings")]
    [Export] private float _bobSpeed = 5.0f;
    [Export] private float _bobAmount = 0.08f;
    [Export] private float _bobSmooth = 5.0f;
    [Export] private float _bobSpeedCap = 6.0f;

    private float _bobTimer;
    private Vector3 _bobCurrentPos;
    private Vector3 _baseLocalPos;


    // Head Shake Settings
    [ExportGroup("Head Shake Idle Settings")]
    [Export] private bool _shakeIdleEnabled = true;
    [Export] private float _shakeIdleSpeed = 1.5f;
    [Export] private float _shakeIdleAmount = 0.002f;

    [ExportGroup("Head Shake Event Settings")]
    [Export] private bool _shakeEventEnabled = true;
    [Export] private float _shakeTraumaDecay = 1.5f;
    [Export] private Vector3 _shakeMaxOffset = new Vector3(0.1f, 0.1f, 0.1f);
    [Export] private Vector3 _shakeMaxRotation = new Vector3(0.05f, 0.05f, 0.05f);

    private float _shakeCurrentTraumaEaseIn = 10.0f;
    private float _shakeCurrentTraumaEaseOut = 1.5f;
    private float _shakeHoldTimer = 0.0f;
    private float _shakeTargetTrauma = 0.0f;

    private float _shakeTrauma = 0.0f;
    private float _shakeTraumaCounter = 0.0f;
    private Vector3 _shakeRot;
    private Vector3 _shakePos;




    public override void _Ready()
    {
        _baseLocalPos = Position;
    }

    public override void _Input(InputEvent @event)  // Get mouse input
    {
        if (_cameraLocked || _player == null)
            return;

        float sensX = _mouseSensitivityX;
        float sensY = _mouseSensitivityY;

        if (_player.CurrentActionState == Player.PlayerActionState.Zoom)
        {
            sensX *= _mouseSensitivityXZoom;
            sensY *= _mouseSensitivityYZoom;
        }

        if (@event is InputEventMouseMotion motion)
        {
            _yaw -= motion.Relative.X * sensX;
            _pitch -= motion.Relative.Y * sensY;

            _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);
        }
    }



    // ================================== //
    // PRE CALCULATIONS
    // ================================== //
    private void CalculateFov(float delta) // Based on player state
    {
        if (_fovLocked)
            return;

        var fovTarget = _fovDefault;
        float speed = _fovSmooth;

        switch (_player.CurrentMoveState)
        {
            case Player.PlayerMoveState.Sprint:
                fovTarget = _fovSprint;
                break;

            case Player.PlayerMoveState.Crouch:
                fovTarget = _fovCrouch;
                break;

            case Player.PlayerMoveState.Jump:
                fovTarget = _fovJump;
                speed = _fovJumpSmooth;
                break;

            case Player.PlayerMoveState.Fall:
                fovTarget = _fovFall;
                speed = _fovFallSmooth;
                break;
        }

        if (_player.CurrentActionState == Player.PlayerActionState.Zoom)
        {
            fovTarget = _fovZoom;
        }

        _playerCamera.Fov = Mathf.Lerp(_playerCamera.Fov, fovTarget, speed * delta);
    }

    private void CalculateRotation(float delta)
    {
        if (_cameraLocked)
            return;

        _smoothYaw = Mathf.LerpAngle(_smoothYaw, _yaw, _cameraSmooth * delta);
        _smoothPitch = Mathf.Lerp(_smoothPitch, _pitch, _cameraSmooth * delta);
    }

    private void CalculateTilt(float delta, Vector3 velocity, bool onFloor)
    {
        float fallSpeed = -velocity.Y;
        Basis b = _player.GlobalTransform.Basis;
        float localVelocityZ = velocity.Dot(b.Z);
        float localVelocityX = velocity.Dot(b.X);

        // Detect fall


        if (!onFloor)
        {
            if (!_fallSideDecided && fallSpeed > _fallBigThreshold)
            {
                _tiltFallSide = Mathf.Abs(_tiltZ) > 0.01f ? Mathf.Sign(_tiltZ) : (GD.Randf() > 0.5f ? 1f : -1f);
                _fallSideDecided = true;
            }

            _fallPeakSpeed = Mathf.Max(_fallPeakSpeed, fallSpeed);
        }

        // Landing
        if (!_wasOnFloor && onFloor)
        {
            float snap;

            if (_fallPeakSpeed > _fallBigThreshold) // Big fall
            {
                float normalized = Mathf.Clamp((_fallPeakSpeed - _fallBigThreshold) / 20f, 0f, 1f);
                snap = normalized * _landImpact;

                _landingTiltX = -snap;
                _landingTiltZ = snap * (Mathf.Abs(_tiltZ) > 0.02f ? Mathf.Sign(_tiltZ) : 0f);
            }
            else if (_fallPeakSpeed > _fallSmallThreshold) // Normal fall
            {
                float normalized = Mathf.Clamp((_fallPeakSpeed - _fallSmallThreshold) / 10f, 0f, 1f);
                snap = Mathf.Min(normalized * _landImpact * 0.4f, _tiltJumpMax);

                _landingTiltX = -snap;
                _landingTiltZ = 0f;
            }

            _fallPeakSpeed = 0f;
            _fallSideDecided = false;
            _tiltFallSide = 0f;
        }

        // Snap recovery
        _landingTiltX = Mathf.Abs(_landingTiltX) < 0.001f ? 0f : Mathf.Lerp(_landingTiltX, 0f, 8f * delta);
        _landingTiltZ = Mathf.Abs(_landingTiltZ) < 0.001f ? 0f : Mathf.Lerp(_landingTiltZ, 0f, 8f * delta);

        // Movement tilt
        float moveTiltX = Mathf.Clamp(-localVelocityZ, -1f, 1f) * _tiltMoveAmount;
        float moveTiltZ = Mathf.Clamp(-localVelocityX, -1f, 1f) * _tiltMoveAmountStrafe;

        // Fall tilt
        float fallTiltX = 0f;
        float fallTiltZ = 0f;

        if (fallSpeed > _fallBigThreshold)
        {
            float rawFall = (fallSpeed - _fallBigThreshold) * _tiltFallAmount;
            float clampedFall = _tiltFallMax * (rawFall / (rawFall + _tiltFallMax));

            float swayX = 0f;
            float swayZ = 0f;

            // Fall Sway
            if (_fallSwayEnabled && !onFloor)
            {
                _bobTimer += delta * _fallSwaySpeed;

                swayX = Mathf.Sin(_bobTimer) * _fallSwayAmount * clampedFall;
                swayZ = Mathf.Cos(_bobTimer * 0.8f) * _fallSwayAmount * clampedFall;
            }

            fallTiltX = clampedFall + swayX;
            fallTiltZ = (clampedFall * _tiltFallSide) + swayZ;
        }

        // Combine
        float targetTiltX = moveTiltX + fallTiltX + _landingTiltX;
        float targetTiltZ = moveTiltZ + fallTiltZ + _landingTiltZ;

        _tiltX = Mathf.Lerp(_tiltX, targetTiltX, _tiltSmooth * delta);
        _tiltZ = Mathf.Lerp(_tiltZ, targetTiltZ, _tiltSmooth * delta);

        // Save micro calculations on idle
        if (onFloor && targetTiltX == 0f && Math.Abs(_tiltX) < 0.0001f)
            _tiltX = 0f;
        if (onFloor && targetTiltZ == 0f && Math.Abs(_tiltZ) < 0.0001f)
            _tiltZ = 0f;
    }

    private void CalculateHeadBob(float delta, float speed, bool onFloor)
    {
        float intensity = Mathf.Clamp(speed / _bobSpeedCap, 0f, 1f);

        if (onFloor && speed > 0.1f)
        {
            _bobTimer += delta * speed * _bobSpeed;
            if (_bobTimer > Mathf.Pi * 4f) // Avoid infinity
            {
                _bobTimer -= Mathf.Pi * 4f;
            }

            float targetBobY = Mathf.Sin(_bobTimer) * _bobAmount * intensity;
            float targetBobX = Mathf.Cos(_bobTimer * 0.5f) * _bobAmount * 0.5f * intensity;

            _bobCurrentPos.Y = Mathf.Lerp(_bobCurrentPos.Y, targetBobY, _bobSmooth * delta);
            _bobCurrentPos.X = Mathf.Lerp(_bobCurrentPos.X, targetBobX, _bobSmooth * delta);
        }
        else
        {
            _bobCurrentPos = _bobCurrentPos.Lerp(Vector3.Zero, _bobSmooth * delta);

            if (onFloor && _bobCurrentPos.LengthSquared() < 0.0001f) // Reset bob
            {
                _bobCurrentPos = Vector3.Zero;
                _bobTimer = 0f;
            }
        }
    }



    // ================================== //
    // CUSTOM CALCULATIONS
    // ================================== //
    public void EventShake(float intensity, float duration, float easeIn = 100f, float easeOut = 1.5f)
    {
        if (!_shakeEventEnabled)
            return;

        _shakeTargetTrauma = Mathf.Clamp(intensity, 0.0f, 1.0f);
        _shakeHoldTimer = duration;
        _shakeCurrentTraumaEaseIn = easeIn;
        _shakeCurrentTraumaEaseOut = easeOut;

        if (easeIn >= 100f)
        {
            _shakeTrauma = _shakeTargetTrauma;
        }
    }

    private void CalculateShake(float delta)
    {
        _shakeTraumaCounter += delta;

        // Handle Idle shake (breathing, idle, sway)
        Vector3 idleRotation = Vector3.Zero;
        if (_shakeIdleEnabled)
        {
            idleRotation.X = Mathf.Sin(_shakeTraumaCounter * _shakeIdleSpeed) * _shakeIdleAmount;
            idleRotation.Z = Mathf.Cos(_shakeTraumaCounter * _shakeIdleSpeed * 0.7f) * _shakeIdleAmount;
        }

        // Handle Event shake
        if (_shakeEventEnabled)
        {
            if (_shakeHoldTimer > 0)
            {
                // Buildup or hold
                _shakeTrauma = Mathf.MoveToward(_shakeTrauma, _shakeTargetTrauma, _shakeCurrentTraumaEaseIn * delta);

                // Count down
                if (Mathf.Abs(_shakeTrauma - _shakeTargetTrauma) < 0.01f)
                {
                    _shakeHoldTimer -= delta;
                }
            }
            else
            {
                // Easing Out
                _shakeTargetTrauma = 0.0f;
                _shakeTrauma = Mathf.MoveToward(_shakeTrauma, 0.0f, _shakeCurrentTraumaEaseOut * delta);
            }
        }
        else
        {
            _shakeTrauma = 0.0f;
        }

        Vector3 eventRotation = Vector3.Zero;
        Vector3 eventTranslation = Vector3.Zero;

        if (_shakeEventEnabled && _shakeTrauma > 0)
        {
            float shakeIntensity = _shakeTrauma * _shakeTrauma;

            float noiseX = Mathf.Sin(_shakeTraumaCounter * 45.0f) * Mathf.Cos(_shakeTraumaCounter * 33.0f);
            float noiseY = Mathf.Sin(_shakeTraumaCounter * 38.0f) * Mathf.Cos(_shakeTraumaCounter * 41.0f);
            float noiseZ = Mathf.Sin(_shakeTraumaCounter * 52.0f) * Mathf.Cos(_shakeTraumaCounter * 27.0f);

            eventTranslation = new Vector3(
                _shakeMaxOffset.X * noiseX,
                _shakeMaxOffset.Y * noiseY,
                _shakeMaxOffset.Z * noiseZ
            ) * shakeIntensity;

            eventRotation = new Vector3(
                _shakeMaxRotation.X * noiseY, // swap because veriaty
                _shakeMaxRotation.Y * noiseZ,
                _shakeMaxRotation.Z * noiseX
            ) * shakeIntensity;
        }

        if (_shakeTrauma <= 0.001f && _shakeTargetTrauma == 0)
        {
            _shakeTrauma = 0;
        }

        _shakePos = eventTranslation;
        _shakeRot = idleRotation + eventRotation;
    }

    //! NEW FUNCTION ON THIS LINE



    // ================================== //
    // APPLY, add new function calculations HERE
    // ================================== //
    private void Apply()
    {
        // Actual player yaw rotation
        _player.Rotation = new Vector3(
            0,
            _smoothYaw + _shakeRot.Y,
            0);

        // Player Head rotation
        Rotation = new Vector3(
            _smoothPitch + _tiltX + _shakeRot.X,
            0,
            _tiltZ + _shakeRot.Z
        );

        // Player Head bob
        Position = _baseLocalPos + _bobCurrentPos + _shakePos;
    }



    public override void _Process(double delta)
    {
        if (_player == null)
            return;

        float d = (float)delta;
        bool onFloor = _player.IsOnFloor();
        Vector3 velocity = _player.Velocity;
        float speed = Mathf.Sqrt(velocity.X * velocity.X + velocity.Z * velocity.Z);

        // Pre calculations
        CalculateFov(d);
        CalculateRotation(d);
        CalculateTilt(d, velocity, onFloor);
        CalculateHeadBob(d, speed, onFloor);

        // Custom calculations
        CalculateShake(d);

        // Apply and reset
        Apply();
        _wasOnFloor = onFloor;
    }
}

