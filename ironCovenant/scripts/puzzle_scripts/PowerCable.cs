using Godot;
using System;

public partial class PowerCable : Node3D
{
	[Export] private StandardMaterial3D mat;
	private bool isOpen = false;
	public override void _Ready()
	{
		mat.EmissionEnabled = false;
	}

	public void OnGetPower()
	{
		if (!isOpen)
		{
			mat.EmissionEnabled = true;
			isOpen = true;
		}
	}

	public void OnStopPower()
	{
		if (isOpen)
		{
			mat.EmissionEnabled = false;
			isOpen = false;
		}
	}
}
