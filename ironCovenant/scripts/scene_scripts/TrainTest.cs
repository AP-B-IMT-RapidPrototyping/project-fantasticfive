using Godot;
using System;

public partial class TrainTest : Node3D
{
	[Export] AnimationPlayer anim;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
		anim.Play("move");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
