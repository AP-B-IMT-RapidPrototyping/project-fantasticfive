using Godot;
using System;
using System.Runtime.Serialization;

public partial class ElectricalDoor : Node3D
{
	[Export] private AnimationPlayer anim;
	private bool isOpen = false;
	public override void _Ready()
	{
		anim.Play("close");
	}

	public void OnGetPower()
	{
		if (!isOpen)
		{
			anim.Play("open");
			isOpen = true;
		}
	}

	public void OnStopPower()
	{
		if (isOpen)
		{
			anim.Play("close");
			isOpen = false;
		}
	}
	
}
