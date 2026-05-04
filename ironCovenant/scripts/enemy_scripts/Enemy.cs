using Godot;
using System;

public partial class Enemy : CharacterBody3D
{
    [Signal] public delegate void EnemyDiedEventHandler();


    [Export] public float visionRange = 15f;
    [Export] public float visionAngle = 90f;

    [Export] public int damage = 20;
    [Export] public float fireRate = 1f;
    [Export] public float attackRange = 5f;
    [Export] public float attackRangePercent = 10; 

    [Export] public float speed = 5f;
    [Export] public int maxHealth = 100;
    [Export] public float gravity = 9.8f;

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
        currentHealth = maxHealth;
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
            velocity.Y -= gravity * (float)delta;
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

                    float targetDistance = attackRange * (1f - (attackRangePercent / 100f));
                    //GD.Print($"dis: {distance} tardis: {targetDistance}");

                    if (distance > targetDistance)
                    {
                        // te ver
                        velocity.X = direction.X * speed;
                        velocity.Z = direction.Z * speed;
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
        if (distance > visionRange)
            return false;

        Vector3 forward = -Transform.Basis.Z;
        Vector3 toPlayer = (player.GlobalPosition - GlobalPosition).Normalized();

        float dot = forward.Dot(toPlayer);
        float threshold = Mathf.Cos(Mathf.DegToRad(visionAngle / 2f));

        return dot > threshold;
    }

    private void HandleAttack(float delta, float distance, bool canSee)
    {
        if (!canSee || distance > attackRange)
            return;

        fireCooldown -= delta;

        if (fireCooldown <= 0f)
        {
            Attack();
            fireCooldown = 1f / fireRate;
        }
    }

    private void Attack()
    {
        // Enemy start animation
        GD.Print("Enemy attacks!");
        anim.Play("Action");
        // Enemy attack anim is on player, player take damage
    }

    public virtual void TakeDamage(int dmg)
    {
        spark.Emitting = true;
        GD.Print("Hitted!");
        currentHealth -= dmg;

        if (currentHealth <= 0)
            Die();
    }

    public void DealDamage()
    {
        float distance = GlobalPosition.DistanceTo(player.GlobalPosition);

        if (distance > attackRange)
        {
            GD.Print("out of range");
            return;
        }

        GD.Print("Hit!");
    
        player.TakeDamage(damage);
    }

    private void Die()
    {
        EmitSignal(SignalName.EnemyDied);
        QueueFree();
    }
}