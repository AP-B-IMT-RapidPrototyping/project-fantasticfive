using Godot;
using System;
using System.Text;

public partial class Flare : RigidBody3D, IInteractable
{
    [Export] public ItemData Item { get; set; }
    public int Amount { get; set; } = 1;

    private uint _collisionLayer;
    private uint _collisionMask;

    [Export] private AnimationPlayer _anim;

    private bool alreadyLighted = false;


    public override void _Ready()
    {
        _collisionLayer = CollisionLayer;
        _collisionMask = CollisionMask;
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
        Throw();
    }

    public void AltUse()
    {
        Light();
    }


    private void Throw()
    {
        GD.Print("Throw Flare");

        var player = GetTree().GetFirstNodeInGroup("player") as Node3D;
        var playerInteract = player.GetNode<PlayerInteract>("PlayerHead/PlayerInteract");

        var throwDir = -GlobalTransform.Basis.Z.Normalized();

        throwDir = throwDir.Normalized();
        playerInteract.DropItem(Item);

        LinearVelocity = throwDir * 15.0f;

        Light();
    }

    private void Light()
    {
        if (!alreadyLighted)
        {
            _anim.Play("play");
            alreadyLighted = true;
        }
    }
}
