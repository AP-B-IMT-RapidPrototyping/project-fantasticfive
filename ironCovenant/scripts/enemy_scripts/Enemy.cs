using Godot;
using System;

public partial class Enemy : CharacterBody3D
{
    [Export] public float VisionRange = 15f;
    [Export] public float VisionAngle = 90f; // NIEUW
    [Export] public float AttackRange = 5f;
    [Export] public float FireRate = 1f;
    [Export] public float Speed = 5f;
    [Export] public int MaxHealth = 100;
    [Export] public float Gravity = 9.8f;

    private int _currentHealth;
    private Node3D _player;
    private float _fireCooldown = 0f;

    private enum State
    {
        Idle,
        Walking
    }

    private State _currentState = State.Idle;

    public override void _Ready()
    {
        _currentHealth = MaxHealth;
        _player = GetTree().GetFirstNodeInGroup("player") as Node3D;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_player == null) return;

        float distance = GlobalPosition.DistanceTo(_player.GlobalPosition);
        bool canSee = CanSeePlayer(distance);

        // 👇 AANGEPAST
        _currentState = canSee ? State.Walking : State.Idle;

        Vector3 velocity = Velocity;

        if (!IsOnFloor())
        {
            velocity.Y -= Gravity * (float)delta;
        }
        else
        {
            velocity.Y = -0.1f;
        }

        switch (_currentState)
        {
            case State.Idle:
                velocity.X = 0;
                velocity.Z = 0;
                break;

            case State.Walking:
                Vector3 direction = (_player.GlobalPosition - GlobalPosition);
                direction.Y = 0;
                direction = direction.Normalized();

                float speedMultiplier = (distance <= AttackRange) ? 0.7f : 1f;

                velocity.X = direction.X * Speed * speedMultiplier;
                velocity.Z = direction.Z * Speed * speedMultiplier;

                LookAt(new Vector3(_player.GlobalPosition.X, GlobalPosition.Y, _player.GlobalPosition.Z), Vector3.Up);
                break;
        }

        // 👇 AANGEPAST
        HandleAttack((float)delta, distance, canSee);

        Velocity = velocity;
        MoveAndSlide();
    }

    // 👇 NIEUW
    private bool CanSeePlayer(float distance)
    {
        if (distance > VisionRange)
            return false;

        Vector3 forward = -Transform.Basis.Z;
        Vector3 toPlayer = (_player.GlobalPosition - GlobalPosition).Normalized();

        float dot = forward.Dot(toPlayer);
        float threshold = Mathf.Cos(Mathf.DegToRad(VisionAngle / 2f));

        return dot > threshold;
    }

    // 👇 AANGEPAST
    private void HandleAttack(float delta, float distance, bool canSee)
    {
        if (!canSee || distance > AttackRange)
            return;

        _fireCooldown -= delta;

        if (_fireCooldown <= 0f)
        {
            Attack();
            _fireCooldown = 1f / FireRate;
        }
    }

    private void Attack()
    {
        GD.Print("Enemy attacks!");
    }

    public void TakeDamage(int dmg)
    {
        GD.Print("Hitted!");
        _currentHealth -= dmg;

        if (_currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        QueueFree();
    }
}