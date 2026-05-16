using Godot;
using System;
using System.ComponentModel;

public partial class ScreenStuff : Node3D
{
	[Export] MeshInstance3D fuelMesh;
	[Export] MeshInstance3D teddyMesh;
	private StandardMaterial3D fuelMat;
	private bool fuelMatStatus = false;

	private StandardMaterial3D teddyMat;
	private bool teddyMatStatus = false;

	[Export] private Timer fuelTimer;
	[Export] private Timer teddyTimer;
	public override void _Ready()
	{
		fuelMat = fuelMesh.GetActiveMaterial(0) as StandardMaterial3D;
		fuelTimer.Timeout += ToggleFuelLight;
		teddyMat = teddyMesh.GetActiveMaterial(0) as StandardMaterial3D;
		teddyTimer.Timeout += ToggleTeddyLight;
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

	public void ToggleTeddyLight()
	{
		if (teddyMatStatus)
		{
			teddyMat.EmissionEnabled = false;
			teddyMatStatus = false;
		} else
		{
			teddyMat.EmissionEnabled = true;
			teddyMatStatus = true;
		}
	}

	public void StartTeddyBlinking()
	{
		teddyTimer.Start();
	}
	
	public void StopTeddyBlinking()
	{
		teddyTimer.Stop();
		teddyMat.EmissionEnabled = false;
		teddyMatStatus = false;
	}
}
