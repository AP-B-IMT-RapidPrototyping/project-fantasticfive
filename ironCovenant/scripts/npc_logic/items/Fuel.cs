using Godot;
using System;

public partial class Fuel : RigidBody3D, IInteractable
{
    [Export] public ItemData Item { get; set; }
    public int Amount { get; set; } = 1;

    private uint _collisionLayer;
    private uint _collisionMask;




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
// DO NOTHING
    }

    public void AltUse()
    {
// DO NOTHING
    }
}
