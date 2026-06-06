using Godot;
using System;

public partial class Gun : RigidBody3D, IInteractable
{
    [Export] public ItemData Item { get; set; }
    public int Amount { get; set; } = 1;

    private uint _collisionLayer;
    private uint _collisionMask;

    [Export] private AnimationPlayer _anim;
    [Export] private Timer delayTimer;

    [Export] private int damage = 100;

    private bool hasHit = false;


    [Export] private RayCast3D _gunCast;


    public override void _Ready()
    {
        _collisionLayer = CollisionLayer;
        _collisionMask = CollisionMask;
    }

    public override void _PhysicsProcess(double delta)
    {

    }



    public void OnDropped()
    {
        Freeze = false;
        CollisionLayer = _collisionLayer;
        CollisionMask = _collisionMask;
    }

    public void OnEquipped()
    {
        Freeze = true;
        CollisionLayer = 0;
        CollisionMask = 0;
    }

    public void Use()
    {
        Attack();
    }

    public void AltUse()
    {
        // Heavy attack? Throw? 
        GD.Print($"ALT {Name}");
        return;
    }


    private void Attack()
    {
        GD.Print("attack");
        if (delayTimer.TimeLeft == 0)
        {
            _anim.Play("shoot");

            if (_gunCast.IsColliding())
            {
                var collider = _gunCast.GetCollider() as EvilCube;
                collider.GetDamage(5);
            }
        }
    }

}
