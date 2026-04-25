using Godot;
using System;
using System.Threading.Tasks;

public partial class SceneTransition : CanvasLayer
{
    [Export] private AnimationPlayer _sceneSwitcherAnimation;

    private async void TransitionFade(bool backwards)
    {
        if (backwards)
        {
            _sceneSwitcherAnimation.Play("fade");
            GD.Print("Fade in");
            await ToSignal(_sceneSwitcherAnimation, AnimationPlayer.SignalName.AnimationFinished);
            GD.Print("Fade done.");
        }
        else
        {
            _sceneSwitcherAnimation.PlayBackwards("fade");
            await ToSignal(_sceneSwitcherAnimation, AnimationPlayer.SignalName.AnimationFinished);
            GD.Print("Fade done.");
        }
    }

    private void TransitionInstant()
    {
        GD.Print("Instant done.");
    }
}
