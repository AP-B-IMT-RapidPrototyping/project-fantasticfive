using Godot;
using System;

public partial class Wood : Enemy
{

	private bool hasFallen = false;
	private bool hasBroken = false;
	[Export] private AnimationPlayer woodAnim;

    public override void _PhysicsProcess(double delta)
    {
        
    }


	public void on_body_entered_wood(Node3D body)
	{
		if (body is Player)
		{
			if (!hasFallen)
			{
				hasFallen = true;
				woodAnim.Play("fall");
			}
		}
	}

	public override void TakeDamage(int dmg)
	{
		GD.Print("wood has been attacked");
		if (!hasBroken)
		{
			woodAnim.Play("break");
			hasBroken = true;
		}
	}
}
