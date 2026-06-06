using Godot;
using System;

public partial class Credits : Node3D
{
    private PackedScene mainMenu = GD.Load<PackedScene>("res://scenes/levels/main_menu.tscn");


    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }



    public void AnimatioFinish(StringName anim)
    {
        GD.Print("HEY HEY HEY HOW YDOINGGOG");
        GetTree().ChangeSceneToPacked(mainMenu);
    }
}
