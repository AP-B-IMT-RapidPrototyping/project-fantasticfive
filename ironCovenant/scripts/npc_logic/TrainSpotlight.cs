using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class TrainSpotlight : Node3D
{
	[Export] private Player player;
	public override void _Ready()
	{
		if (player == null)
		{
			GD.Print("No player found");
		}
	}

	
	public override void _Process(double delta)
	{
		if (player != null)
		{
			Vector3 playerLocation = player.GlobalPosition;
			LookAt(playerLocation);
			
		}
	}
}
