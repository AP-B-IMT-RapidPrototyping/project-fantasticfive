using Godot;
using System;

public partial class ChurchLightTrigger : Area3D
{

    [Export] private AnimationPlayer _churchlight;
    private bool alreadyplayed = false;

    private void OnBodyEntered(Node3D body)
    {
        if (!alreadyplayed)
        {
            _churchlight.Play("play");
            alreadyplayed = true;
        }
    }
}
