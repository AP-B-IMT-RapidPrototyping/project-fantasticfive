using Godot;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

public partial class Level0 : Node3D
{
    [Export] private AnimationPlayer _cutsceneIntroAnim;
    [Export] private AnimationPlayer _cutsceneAnim;
    [Export] private AnimationPlayer _cutsceneOutroAnim;

    [Export] private Camera3D _cutsceneCamera;




    private void OnCutsceneTrigger(Node3D body)
    {
        if (body.IsInGroup("player"))
        {
            GD.Print("yes " + body.Name);
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
    }
}
