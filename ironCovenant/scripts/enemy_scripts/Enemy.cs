using Godot;
using System;

public partial class Enemy : CharacterBody3D
{
    [Signal] public delegate void EnemyDiedEventHandler();


    [Export] public float VisionRange = 15f;
    [Export] public float VisionAngle = 90f;

    [Export] public int Damage = 20;
    [Export] public float FireRate = 1f;
    [Export] public float AttackRange = 5f;
    [Export] public float AttackRangePercent = 10; 

    [Export] public float Speed = 5f;
    [Export] public int MaxHealth = 100;
    [Export] public float Gravity = 9.8f;

    [Export] private AnimationPlayer anim;

    [Export] private GpuParticles3D spark;

    private int currentHealth;
    private Player player;
    private float fireCooldown = 0f;

    private enum State
    {
        Idle,
        Walking
    }

    private State currentState = State.Idle;

    public override void _Ready()
    {
        currentHealth = MaxHealth;
        player = GetTree().GetFirstNodeInGroup("player") as Player;

        
    }

    public override void _PhysicsProcess(double delta)
    {
        if (player == null) 
        {
            GD.Print("Player not found");
        }

        float distance = GlobalPosition.DistanceTo(player.GlobalPosition);
        bool canSee = CanSeePlayer(distance);

        currentState = canSee ? State.Walking : State.Idle;

        Vector3 velocity = Velocity;

        if (!IsOnFloor())
        {
            velocity.Y -= Gravity * (float)delta;
        }
        else
        {
            velocity.Y = -0.1f;
        }

        switch (currentState)
        {
            case State.Idle:
                anim.Play("Idle");
                velocity.X = 0;
                velocity.Z = 0;
                break;

            case State.Walking:
                if (!(anim.CurrentAnimation == "Action"))
                {
                    anim.Play("walk");

                    Vector3 direction = (player.GlobalPosition - GlobalPosition);
                    direction.Y = 0;
                    direction = direction.Normalized();

                    float targetDistance = AttackRange * (1f - (AttackRangePercent / 100f));
                    //GD.Print($"dis: {distance} tardis: {targetDistance}");

                    if (distance > targetDistance)
                    {
                        // te ver
                        velocity.X = direction.X * Speed;
                        velocity.Z = direction.Z * Speed;
                    }
                    else
                    {
                        // in range
                        velocity.X = 0;
                        velocity.Z = 0;
                    }
                }
                LookAt(
                    new Vector3(player.GlobalPosition.X, GlobalPosition.Y, player.GlobalPosition.Z),
                    Vector3.Up
                );

                break;
        }

        HandleAttack((float)delta, distance, canSee);

        

        Velocity = velocity;
        MoveAndSlide();
    }

    private bool CanSeePlayer(float distance)
    {
        if (distance > VisionRange)
            return false;

        Vector3 forward = -Transform.Basis.Z;
        Vector3 toPlayer = (player.GlobalPosition - GlobalPosition).Normalized();

        float dot = forward.Dot(toPlayer);
        float threshold = Mathf.Cos(Mathf.DegToRad(VisionAngle / 2f));

        return dot > threshold;
    }

    private void HandleAttack(float delta, float distance, bool canSee)
    {
        if (!canSee || distance > AttackRange)
            return;

        fireCooldown -= delta;

        if (fireCooldown <= 0f)
        {
            Attack();
            fireCooldown = 1f / FireRate;
        }
    }

    private void Attack()
    {
        // Enemy start animation
        GD.Print("Enemy attacks!");
        anim.Play("Action");
        // Enemy attack anim is on player, player take damage
    }

    public void TakeDamage(int dmg)
    {
        spark.Emitting = true;
        GD.Print("Hitted!");
        currentHealth -= dmg;

        if (currentHealth <= 0)
            Die();
    }

    public void DealDamage()
    {
        GD.Print("Player Damaged");
        //if ()
        player.TakeDamage(Damage);
    }

    private void Die()
    {
        EmitSignal(SignalName.EnemyDied);
        QueueFree();
    }
}