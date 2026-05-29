using Godot;
using System;

public partial class Level4 : Node3D
{
	[Export] private TrainSpotlight spot;
	[Export] private Church door;

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
}
