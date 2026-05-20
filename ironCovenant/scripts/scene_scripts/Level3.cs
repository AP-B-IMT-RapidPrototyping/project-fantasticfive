using Godot;
using System;

public partial class Level3 : Node3D
{
	[Export] private TrainSpotlight spotlight;
	[Export] private ScreenStuff screen;
	public override void _Ready()
	{
		spotlight.canRadiate = true;
		screen.StartTeddyBlinking();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
