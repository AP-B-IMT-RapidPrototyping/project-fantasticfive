using Godot;
using System;
using System.ComponentModel;

public partial class ScreenStuff : Node3D
{
	[Export] MeshInstance3D fuelMesh;
	private StandardMaterial3D fuelMat;
	private bool fuelMatStatus = false;
	[Export] private Timer fuelTimer;
	public override void _Ready()
	{
		fuelMat = fuelMesh.GetActiveMaterial(0) as StandardMaterial3D;
		fuelTimer.Timeout += ToggleFuelLight;
	}

	public void ToggleFuelLight()
	{
		if (fuelMatStatus)
		{
			fuelMat.EmissionEnabled = false;
			fuelMatStatus = false;
		} else
		{
			fuelMat.EmissionEnabled = true;
			fuelMatStatus = true;
		}
	}

	public void StartFuelBlinking()
	{
		fuelTimer.Start();
	}
	
	public void StopFuelBlinking()
	{
		fuelTimer.Stop();
		fuelMat.EmissionEnabled = false;
		fuelMatStatus = false;
	}
}
