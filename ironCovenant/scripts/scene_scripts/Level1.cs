using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class Level1 : Node3D
{
	[Export] private TrainSpotlight spotlight;
	[Export] private ItemData axe;
	[Export] private AudioStreamPlayer3D trainNoise;
	[Export] private AudioStreamPlayer3D evilCubeNoise;
	[Export] private PlayerHead playerHead;
	[Export] private Player player;
	[Export] private Marker3D cubeMarker;
	[Export] private Timer startRunTimer;
	[Export] private Timer trainWheelsTimer;
	[Export] private Timer bufferTimer;
	[Export] private Timer deathTimer;
	[Export] private MeshInstance3D evilCube;
	[Export] private AnimationPlayer chaseAnim;
	[Export] private AnimationPlayer trainAnim;
	[Export] private Enemy defaultEnemy;
	[Export] private CanvasLayer deathLayer;

	private bool chaseCanStart = false;
	private bool playerCanDie = false;
	private Node _sceneManager = null;

    public override void _Ready()
    {
        startRunTimer.Timeout += StopLook;
		bufferTimer.Timeout += StartRun;
		trainWheelsTimer.Timeout += StartWheels;
		_sceneManager = GetNode("/root/SceneManager");

        _sceneManager.Call("RegisterAreas");

		deathLayer.Visible = false;
    }

	private void On_Enemy_Died()
	{
		trainNoise.Play();
	}


	private void on_body_entered_spotlightArea(Node3D body)
	{
		if (body is Player)
		{
			var items = InventorySystem.Inventory.GetItems();
			if (items.ContainsKey(axe))
			{
				spotlight.canRadiate = true;
				spotlight.StartRadiation();
				trainNoise.Play();
				chaseCanStart = true;
				defaultEnemy.Visible = true;
				defaultEnemy.GlobalPosition = new Vector3(8.255f, 1.317f, 46.773f);
				//defaultEnemy.Rotation = new Vector3(Mathf.DegToRad(46.1f), Mathf.DegToRad(57.0f), Mathf.DegToRad(-36.9f));
			}
		}
	}

	private void on_body_entered_chaseArea(Node3D body)
	{
		if (body is Player && chaseCanStart)
		{
			//start chase
			GD.Print("chase start");
			chaseCanStart = false;
			playerCanDie = true;
			evilCube.Visible = true;
			chaseAnim.Play("chase_scene");
			playerHead.CameraLookAt(cubeMarker);
			playerHead.haveToLookAt = true;
			player.canMove = false;
			evilCubeNoise.Play();
			startRunTimer.Start();
			trainWheelsTimer.Start();
		}
	}

	private void on_body_entered_deathArea(Node3D body)
	{
		if (body is Player && playerCanDie)
		{
			playerHead._cameraLocked = true;
			chaseAnim.Pause();
			deathLayer.Visible = true;
			deathTimer.Timeout += DieAftermath;
			deathTimer.Start();
		}
	}

	private void DieAftermath()
	{
		deathTimer.Timeout -= DieAftermath;
		playerHead._cameraLocked = false;
		deathLayer.Visible = false;
		Vector3 respawnPos = new Vector3(-4.173f, 1.052f, 20.786f);
		player.GlobalPosition = respawnPos;
		playerHead._yaw = Mathf.DegToRad(0f);
		chaseCanStart = true;
		chaseAnim.Stop();
		on_body_entered_chaseArea(player);
	}

	private void StopLook()
	{
		playerHead.haveToLookAt = false;
		bufferTimer.Start();
	}

	private void StartRun()
	{
		player.canMove = true;
	}

	private void StartWheels()
	{
		GD.Print("wheels started");
		trainAnim.Play("move");
	}

}
