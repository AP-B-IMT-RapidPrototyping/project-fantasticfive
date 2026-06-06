using Godot;
using System;

public partial class EvilCube : Node3D
{
    [Export] private AnimationPlayer _dieAnim;

    private PackedScene _creditsScene = GD.Load<PackedScene>("res://scenes/levels/credits.tscn");



    private int _maxHealth = 100;
    private int _health = 100;

    public void GetDamage(int dmg)
    {
        if (_health > 0)
        {
            _health -= dmg;
        }
        else
        {
            Die();
        }
    }


    public void Die()
    {
        _dieAnim.Play("play");
    }


    public void DeathAnimFinished(StringName anim)
    {
        GetTree().ChangeSceneToPacked(_creditsScene);
    }

}
