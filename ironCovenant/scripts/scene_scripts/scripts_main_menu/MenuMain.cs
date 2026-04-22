using Godot;
using System;

public partial class MenuMain : Node3D
{
    [Export] private AnimationPlayer _animIntro;
    [Export] private AnimationPlayer _animWheels;

    public override void _Ready()
    {
        _animIntro.AnimationFinished += onAnimationFinished;
        _animWheels.Play("move");
    }

    /* skip intro */
    public override void _UnhandledInput(InputEvent @event){
        if (@event.IsActionPressed("jump"))
        {
                _animIntro.Seek(_animIntro.GetAnimation(_animIntro.CurrentAnimation).Length, true);
        }
    }
    
    public void onAnimationFinished(StringName animName)
    {
        _animWheels.Pause();
    }
}
