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

	private double intensity = 0;
	private double baseIntensity = 1;
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
				Vector3 origin = shapeCast.GlobalTransform.Origin;

				for (int i = 0; i < shapeCast.GetCollisionCount(); i++)
				{
					var collider = shapeCast.GetCollider(i);
					Vector3 point = shapeCast.GetCollisionPoint(i);

					float dist = origin.DistanceTo(point);

					results.Add((collider, dist));
				}

				results.Sort((a, b) => a.distance.CompareTo(b.distance));

				if (results[0].collider is Node3D node)
				{
					if (node.Name == "Player" || node.Name == "TrainCollision" || node.Name == "Floor")
					{
						//player is infront
						if (beingRadiated)
						{
							StopRadiation();
							beingRadiated = false;
						}
					}
					else
					{
						//player is behind
						if (!beingRadiated)
						{
							beingRadiated = true;
							bufferTimer.Start();
						}
					}
				}
			}
		}
	}

	private void StartRadiation()
	{
		radiationTick.Start();
		intensity = baseIntensity;
		_shader.SetShaderParameter("intensity", intensity);
		_shaderRect.Visible = true;
		GD.Print("Start radiation");
	}

	private void StopRadiation()
	{
		radiationTick.Stop();
		bufferTimer.Stop();
		_shaderRect.Visible = false;
		GD.Print("Stop radiation");
	}

	private void AddRadiation()
	{
		intensity += 0.01;
		_shader.SetShaderParameter("intensity", intensity);
	}
}
