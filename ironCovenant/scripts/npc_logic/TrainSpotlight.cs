using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public partial class TrainSpotlight : Node3D
{
	[Export] private Player player;
	[Export] private Timer visionTimer;
	[Export] private ShapeCast3D shapeCast;

	public override void _Ready()
	{
		visionTimer.Timeout += CheckVision;
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

	private void _on_child_entered_tree(Node node)
	{
		GD.Print($"Child entered {node.Name}");
	}

	private void CheckVision()
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
					GD.Print("Player is infront");
				} else
				{
					GD.Print($"Player is not infront, blocked by {node.Name}");
				}
			}
		}
	}
}
