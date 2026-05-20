using Godot;
using System;

public partial class Level2 : Node3D
{
	[Export] private TrainSpotlight spotlight;
	[Export] private ScreenStuff screen;
	[Export] private ElectricalDoor door;

	private Node _sceneManager = null;

	public override void _Ready()
	{
		spotlight.canRadiate = true;
		screen.StartFuelBlinking();
		_sceneManager = GetNode("/root/SceneManager");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void onBodyEnteredFireArea(Node3D body)
	{
		if (body is Player)
		{
			door.OpenDoorPermanently();
		}
	}
}
