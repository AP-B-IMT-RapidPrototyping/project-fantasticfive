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
	[Export] private PlayerHead playerHead;
	[Export] private Player player;
	[Export] private Marker3D cubeMarker;
	[Export] private Timer startRunTimer;
	[Export] private MeshInstance3D evilCube;
	[Export] private AnimationPlayer chaseAnim;

	private bool chaseCanStart = true;

    public override void _Ready()
    {
        startRunTimer.Timeout += StartRun;
    }

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
			chaseCanStart = false;
			evilCube.Visible = true;
			chaseAnim.Play("chase_scene");
			playerHead.CameraLookAt(cubeMarker);
			playerHead.haveToLookAt = true;
			player.canMove = false;
			startRunTimer.Start();
		}
	}

	private void StartRun()
	{
		playerHead.haveToLookAt = false;
		player.canMove = true;
	}
}
