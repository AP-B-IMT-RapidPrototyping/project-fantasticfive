using Godot;
using System;

public partial class MenuMain : Node3D
{
    [Export] private AnimationPlayer _animIntro;


    /* skip intro */
    public override void _UnhandledInput(InputEvent @event){
        if (@event.IsActionPressed("jump"))
        {
                _animIntro.Seek(_animIntro.GetAnimation(_animIntro.CurrentAnimation).Length, true);
        }
    }
}
