using Godot;
using System;

public partial class Train : Node3D
{
	[Export] private AnimationPlayer _anim;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_anim.Play("closeBox");
	}

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
