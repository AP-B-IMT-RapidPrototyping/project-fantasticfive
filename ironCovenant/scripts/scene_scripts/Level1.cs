using Godot;
using System;

public partial class Level1 : Node3D
{
	[Export] private TrainSpotlight spotlight;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//spotlight.canRadiate = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
