using Godot;
using System;

public partial class lightArea : Area3D
{
	[Export] private AnimationPlayer lightAnim;

	public void on_body_entered(Node3D body)
	{
		if (body is Player)
		{
			GD.Print("Area triggered");
			lightAnim.Play("light");
		}
	}
}
