using Godot;
using System;

public partial class Train : Node3D
{
	[Export] private AnimationPlayer _anim;


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OpenBox()
	{
		_anim.Play("openBox");
	}

	public void CloseBox()
	{
		_anim.Play("closeBox");
	}
}
