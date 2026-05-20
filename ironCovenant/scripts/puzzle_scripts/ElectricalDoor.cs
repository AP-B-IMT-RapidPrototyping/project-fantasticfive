using Godot;
using System;
using System.Runtime.Serialization;

public partial class ElectricalDoor : Node3D
{
	[Export] private AnimationPlayer anim;
	private bool isOpen = false;
	public bool permaOpen = false;
	public override void _Ready()
	{
		anim.Play("close");
	}

	public void OnGetPower()
	{
		if (!isOpen && !permaOpen)
		{
			anim.Play("open");
			isOpen = true;
		}
	}

	public void OnStopPower()
	{
		if (isOpen && !permaOpen)
		{
			anim.Play("close");
			isOpen = false;
		}
	}

	public void OpenDoorPermanently()
	{
		if (!isOpen && !permaOpen)
		{
			anim.Play("open");
			isOpen = true;
		}
		permaOpen = true;
	}
	
}
