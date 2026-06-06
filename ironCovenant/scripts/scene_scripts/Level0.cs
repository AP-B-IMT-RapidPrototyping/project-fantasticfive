using Godot;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

public partial class Level0 : Node3D
{
    [Export] private AnimationPlayer _introText;
    private bool _introTextDone = false;

    [Export] private Label _introSkipText;
    private bool _introHasSkippedOnce = false;

    [Export] private Train _train;
    [Export] private AnimationPlayer _cutsceneIntroAnim;
    [Export] private AnimationPlayer _cutsceneAnim;
    [Export] private AnimationPlayer _cutsceneOutroAnim;
    [Export] private AnimationPlayer _cutsceneEnding;


    [Export] private AnimationPlayer _thatOneFuckingPole;


    [Export] private Camera3D _cutsceneCamera;

    


    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("jump") && !_introTextDone)
        {
            if (_introHasSkippedOnce)
            {
                _introText.Seek(_introText.GetAnimation(_introText.CurrentAnimation).Length, true);
                _introTextDone = true;
            }
            else
            {
                _introHasSkippedOnce = true;
                _introSkipText.Text = "are you sure you want to skip? (space)";
            }
        }
    }



    private void OnCutsceneTrigger(Node3D body)
    {
        if (body.IsInGroup("player"))
        {
            _cutsceneCamera.Current = true;
            _cutsceneIntroAnim.Play("play");
        }
    }

    private void OnIntroFinished(StringName anim)
    {
        _cutsceneAnim.Play("play");
    }

    private void OnCutsceneFinished(StringName anim)
    {
        _cutsceneOutroAnim.Play("play");
    }

    private void OnOutroFinished(StringName anim)
    {
        _cutsceneCamera.Current = false; // fix camera switching back to player   
        _train._canCloseDoors = true;
        _thatOneFuckingPole.Stop();
        _thatOneFuckingPole.Play("RESET");
        _thatOneFuckingPole.Active = false;
        _thatOneFuckingPole.Stop();
        GD.Print("me buy the egg");
        GD.Print("Level Done.");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_train._playerInTrain)
        {
            _cutsceneEnding.Play("play");
        }
    }
}
