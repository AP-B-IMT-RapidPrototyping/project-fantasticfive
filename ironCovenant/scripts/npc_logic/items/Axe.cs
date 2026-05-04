using Godot;
using System;

public partial class Axe : RigidBody3D, IInteractable
{
	[Export] public ItemData Item { get; set; }
	public int Amount { get; set; } = 1;

	private uint _collisionLayer;
	private uint _collisionMask;

	[Export] private AnimationPlayer _anim;
	[Export] private Area3D hitArea;
	[Export] private Timer delayTimer;

	[Export] private int damage = 50;

	private bool hasHit = false;

	public override void _Ready()
	{
		_collisionLayer = CollisionLayer;
		_collisionMask = CollisionMask;
	}

    public override void _PhysicsProcess(double delta)
    {
        if ((delayTimer.TimeLeft < 0.4) && (!hasHit))
		{
			hasHit = true;
			foreach (Node3D node in hitArea.GetOverlappingBodies())
				{
					GD.Print($"hit node: {node}");
					if (node.IsInGroup("enemy"))
					{
						GD.Print("enemy hit");
						if (node is Enemy enemy)
						{
							enemy.TakeDamage(damage);
						}
					}
				}
		}
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
			delayTimer.Start();
			hasHit = false;
			if (!_anim.IsPlaying())
			{
				_anim.Play("attack2");
			}
		}
	}

}
