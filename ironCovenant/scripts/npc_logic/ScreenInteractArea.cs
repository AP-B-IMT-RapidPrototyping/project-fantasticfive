using Godot;
using Microsoft.VisualBasic;
using System;

public partial class ScreenInteractArea : StaticBody3D
{

	public void Interact()
	{
		GD.Print("interacted with screen");
	}
}
