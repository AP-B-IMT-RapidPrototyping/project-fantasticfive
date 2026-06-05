using Godot;
using System;

public partial class Church : Node3D
{
	[Export] private AnimationPlayer anim;
	[Export] private AnimationPlayer audioanim;
	private bool isOpen = false;
	public bool permaOpen = false;

	private bool panel1On = false;
	// <summary>
	private bool panel2On = false;
	// </summary>

	public override void _Ready()
	{
		anim.Play("close");
	}

	public void OnGetPower1()
	{
		GD.Print("Get power 1");
		panel1On = true;
		TryOpeningDoor();
	}

	public void OnStopPower1()
	{
		GD.Print("Lose power 1");
		panel1On = false;
		TryClosingDoor();
	}

	public void OnGetPower2()
	{
		GD.Print("Get power 2");
		panel2On = true;
		TryOpeningDoor();
	}

	public void OnStopPower2()
	{
		GD.Print("Lose power 2");
		panel2On = false;
		TryClosingDoor();
	}


	private void TryOpeningDoor()
	{
		GD.Print("Try opening door");
		if (panel1On && panel2On)
		{
			GD.Print("Both on");
			if (!isOpen && !permaOpen)
			{
				anim.Play("open");
				isOpen = true;
			}
		}
	}

	private void TryClosingDoor()
	{
		if (isOpen && !permaOpen)
		{
			anim.Play("close");
			isOpen = false;
		}
	}

	public void OpenDoorPermanently()
	{
		
	}
}
