using Godot;
using System;

public partial class Level4 : Node3D
{
	[Export] private TrainSpotlight spot;
	[Export] private Church door;



	[Export] private ItemData gun;
	private bool _gunPickedUp = false;
	[Export] private AnimationPlayer _bossAnimation;


	public override void _Ready()
	{
		spot.canRadiate = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void onBodyEnteredChurcheArea(Node3D body)
	{
		if (body is Player)
		{
			door.OpenDoorPermanently();
		}
	}



	public override void _PhysicsProcess(double delta)
	{
		// this shouldn't be placed in physicsprocess, or process, its rlly bad practice, but i dont care atm
		if (!_gunPickedUp)
		{
			if (InventorySystem.Inventory == null)
			{
				GD.Print("Inventory not found");
				return;
			}

			if (InventorySystem.Inventory.HasItem(gun, 1))
			{
				GD.Print("boss!");
				_bossAnimation.Play("play");
				_gunPickedUp = true;
			}
		}
	}
}
