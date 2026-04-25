using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class Level1 : Node3D
{
	[Export] private TrainSpotlight spotlight;
	[Export] private InventorySystem invSystem;
	[Export] private ItemData axe;
	[Export] private AudioStreamPlayer3D trainNoise;

	private bool chaseCanStart = false;


	private void on_body_entered_spotlightArea(Node3D body)
	{
		if (body is Player)
		{
			var items = invSystem.GetItems();
			if (items.ContainsKey(axe))
			{
				spotlight.canRadiate = true;
				spotlight.StartRadiation();
				trainNoise.Play();
				chaseCanStart = true;
			}
		}
	}

	private void on_body_entered_chaseArea(Node3D body)
	{
		if (body is Player && chaseCanStart)
		{
			//start chase
			GD.Print("start chase");
		}
	}
}
