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

	[Export] private int damage;

	public override void _Ready()
	{
		_collisionLayer = CollisionLayer;
		_collisionMask = CollisionMask;
	}


	public void OnDropped()
	{
		CollisionLayer = _collisionLayer;
		CollisionMask = _collisionMask;
	}

	public void OnEquipped()
	{
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
		if (delayTimer.TimeLeft == 0)
		{
			delayTimer.Start();
			if (!_anim.IsPlaying())
			{
				_anim.Play("attack2");
			}
			if (_anim.CurrentAnimation == "attack2")
			{
				foreach (Node3D node in hitArea.GetOverlappingBodies())
				{
					if (node.IsInGroup("enemy"))
					{
						GD.Print("enemy hit");
						// if (node is Enemy enemy)
						// {
						// 	enemy.TakeDamage(damage);
						// }
					}
				}
			}
		}
	}
}
