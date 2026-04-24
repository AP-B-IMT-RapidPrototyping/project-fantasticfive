using Godot;
using System;

public partial class Axe : Node3D
{

	[Export] private AnimationPlayer _anim;
	[Export] private Area3D hitArea;
	[Export] private Timer delayTimer;

	[Export] private int damage;

	public override void _Ready()
	{
	}

	
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("attack"))
		{
			if (delayTimer.TimeLeft == 0)
			{
				delayTimer.Start();
				if (!_anim.IsPlaying())
				{
					_anim.Play("attack");
				}
				if (_anim.CurrentAnimation == "attack")
				{
					foreach (Node3D node in hitArea.GetOverlappingBodies())
					{
						if (node.IsInGroup("enemy"))
						{
							GD.Print("enemy hit");
							// if (node is Enemy enemy)
							// {
							// 	enemy.TakeDamage(damage);
							// }
						}
					}
				}
			}
		}
	}
}
