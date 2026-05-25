using Godot;
using Microsoft.VisualBasic;
using System;

public partial class ScreenInteractArea : StaticBody3D
{
	[Signal]
	public delegate void StartTrainEventHandler();
	public void Interact()
	{
		GD.Print("interacted with screen");
		EmitSignal(SignalName.StartTrain);
	}
}
