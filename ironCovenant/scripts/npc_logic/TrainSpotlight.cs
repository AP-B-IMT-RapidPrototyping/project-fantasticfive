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
	[Export] private ShapeCast3D spotCast;
	[Export] private Node3D spotCastTurnPoint;
	[Export] private Area3D spotlightArea;

	[Export] private RayCast3D energyCast; 

	[Export] private ShaderMaterial _shader;
	[Export] private TextureRect _shaderRect;

	[Export] private AudioStreamPlayer geigerMeter;

	public double intensity = 0;
	private double baseIntensity = .99;
	private bool beingRadiated = false;
	public bool canRadiate = false;
	private bool followPlayer = true;

	private bool spotCastCheck = false;
	private bool coneCheck = false;

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

			if (followPlayer)
			{
				LookAt(playerLocation);	
			}

			spotCastTurnPoint.LookAt(playerLocation);
		}

		if (Input.IsActionJustPressed("toggleLight"))
		{
			followPlayer = !followPlayer;
		}
	}

	private void _on_child_entered_tree(Node node)
	{
		GD.Print($"Child entered {node.Name}");
	}

	private void CheckVision()
	{
		var overlaps = spotlightArea.GetOverlappingBodies();
		if (canRadiate)
		{
			//player before or behind an object
			spotCast.ForceShapecastUpdate();
			if (spotCast.IsColliding())
			{
				//GD.Print("ShapeCast is colliding");
				// Maak een lijst van de colliders en sorteer ze in de array op volgorde van afstand tov origin
				var results = new List<(GodotObject collider, float distance)>();
				Node3D _originNode = GetParent() as Node3D;
				Vector3 _origin = _originNode.GlobalPosition;

				for (int i = 0; i < spotCast.GetCollisionCount(); i++)
				{
					var collider = spotCast.GetCollider(i);
					Vector3 point = spotCast.GetCollisionPoint(i);

					float dist = _origin.DistanceTo(point);

					results.Add((collider, dist));
				}

				results.Sort((a, b) => a.distance.CompareTo(b.distance));

				int checkIndex = 0;
				
				//Als de eerste collider de vloer of trein is negeer deze
				if (results[checkIndex].collider is Node3D node)
				{
					if (node.Name == "Floor")
					{
						//GD.Print("First node is floor");
						checkIndex++;
					}
				}

				try
				{
					if (results[checkIndex].collider is Node3D node2)
					{
						if (node2.Name == "Train" || node2.Name == "DoorL" || node2.Name == "DoorR" || node2.Name == "backwallSafety")
						{
							//GD.Print("First node is train");
							checkIndex++;
						}
					}
				} catch
				{
					//GD.Print("There wasn't a second collider");
				}

				//Controlleer of de dichtste collider de speler is
				try
				{
					if (results[checkIndex].collider is Node3D closeNode)
					{
						if (closeNode.Name == "Player")
						{
							//GD.Print("Closest is player");
							spotCastCheck = true;
						} else
						{
							//GD.Print($"Closest isn't player, it is: {closeNode.Name}");
							spotCastCheck = false;
						}
					}
				} catch // mogelijks was de trein of de vloer de enige collider, in elk geval is de speler niet in de shapecast
				{
					//GD.Print("There wasn't a second or third collider");
					spotCastCheck = false;
				}
			} else
			{
				//GD.Print("Didn't collide");
				spotCastCheck = false;
			}

			//GD.Print($"Shapecast check status: {spotCastCheck}");


			//player binnen of buiten spotlight
			coneCheck = false;
			foreach (Node3D body in overlaps)
			{
				if (body is Player)
				{
					coneCheck = true;
				}
			}

			//GD.Print($"Cone check status: {coneCheck}");

			//Alleen als de speler binnen de cone is en als eerste voor de shapecast dan kan de speler niet geradieert worden
			if (coneCheck && spotCastCheck)
			{
				if (beingRadiated)
				{
					StopRadiation();
					beingRadiated = false;
				}
			} else
			{
				if (!beingRadiated)
				{
					beingRadiated = true;
					geigerMeter.Play();
					bufferTimer.Start();
				}
			}
		}

		//check if energy device is visible
		foreach (Node3D body in overlaps)
		{
			//GD.Print($"Checking bodies, current: {body}");
			//Als er een light panel in de light cone zit
			if (body is LightPanel panel)
			{
				//Kijk naar het light panel om te controleren of het paneel zichtbaar is vanaf de trein
				energyCast.LookAt(panel.GlobalPosition);
				energyCast.ForceRaycastUpdate();
				
				//GD.Print($"energy cast collider: ${energyCast.GetCollider()}");
				if (energyCast.GetCollider() is Node3D node)
				{
					if (node.Name == "")
					GD.Print("Panel gets power");
					panel.GetPower();
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

		//GD.Print($"Intensity is now = {intensity}");

		if (intensity >= 1.03)
		{
			player.Die();
		}
	}
}
