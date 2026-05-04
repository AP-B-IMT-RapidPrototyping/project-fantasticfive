using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public partial class TrainSpotlight : Node3D
{
	[Export] private Player player;
	[Export] private Timer visionTimer;
	[Export] private Timer bufferTimer;
	[Export] private Timer radiationTick;
	[Export] private ShapeCast3D shapeCast;

	[Export] private ShaderMaterial _shader;
	[Export] private ColorRect _shaderRect;

	[Export] private AudioStreamPlayer geigerMeter;

	public double intensity = 0;
	private double baseIntensity = .97;
	private bool beingRadiated = false;
	public bool canRadiate = false;

	public override void _Ready()
	{
		visionTimer.Timeout += CheckVision;
		if (player == null)
		{
			GD.Print("No player found");
		}
		_shaderRect.Visible = false;
		bufferTimer.Timeout += StartRadiation;
		radiationTick.Timeout += AddRadiation;
	}

	
	public override void _Process(double delta)
	{
		if (player != null)
		{
			Vector3 playerLocation = player.GlobalPosition;
			LookAt(playerLocation);
			
		}
	}

	private void _on_child_entered_tree(Node node)
	{
		GD.Print($"Child entered {node.Name}");
	}

	private void CheckVision()
	{
		if (canRadiate)
		{
			shapeCast.ForceShapecastUpdate();
			if (shapeCast.IsColliding())
			{
				var results = new List<(GodotObject collider, float distance)>();
				Node3D _originNode = GetParent() as Node3D;
				Vector3 _origin = _originNode.GlobalPosition;

				for (int i = 0; i < shapeCast.GetCollisionCount(); i++)
				{
					var collider = shapeCast.GetCollider(i);
					Vector3 point = shapeCast.GetCollisionPoint(i);

					float dist = _origin.DistanceTo(point);

					results.Add((collider, dist));
				}

				results.Sort((a, b) => a.distance.CompareTo(b.distance));


				if (results[0].collider is Node3D node)
				{
					GD.Print($"Collidor 0: ${node.Name}");
					if (node.Name == "Player" || node.Name == "TrainCollision")
					{
						//player is infront
						if (beingRadiated)
						{
							StopRadiation();
							beingRadiated = false;
						}
					}
					else if (node.Name == "Floor")
					{
						if (results[1].collider is Node3D node2)
						{
							GD.Print($"Collider 1: ${node2}");
							if (node2.Name == "Player")
							{
								if (beingRadiated)
								{
									StopRadiation();
									beingRadiated = false;
								}
							}
						}
					}
					else
					{
						//player is behind
						if (!beingRadiated)
						{
							beingRadiated = true;
							geigerMeter.Play();
							bufferTimer.Start();
						}
					}
				}
			}
		}
	}

	private void StartRadiation()
	{
		beingRadiated = true;
		radiationTick.Start();
		intensity = baseIntensity;
		_shader.SetShaderParameter("intensity", intensity);
		_shaderRect.Visible = true;
		GD.Print("Start radiation");
	}

	private void StopRadiation()
	{
		geigerMeter.Stop();
		radiationTick.Stop();
		bufferTimer.Stop();
		_shaderRect.Visible = false;
		GD.Print("Stop radiation");
	}

	private void AddRadiation()
	{
		intensity += 0.01;
		_shader.SetShaderParameter("intensity", intensity);

		GD.Print($"Intensity is now = {intensity}");

		if (intensity >= 1.03)
		{
			player.Die();
		}
	}
}
