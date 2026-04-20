using Godot;
using System;
using System.Threading.Tasks;

public partial class SceneManager : CanvasLayer
{
    //! THIS SCRIPT IS UNDER DEVELOPMENT AND MAY **NOT** WORK AS INTENDED
    /* Please test the C# version  */
    /* Previously known as scene_man.gd, translated to C# */
    /* --------------------------------------------------------------- */


    //TODO SETUP:
    
    //TODO This script uses group "scene_teleporter_area" to define which Area3D nodes are teleporters
    //TODO and uses the name of the Area3D node to determine which scene to load
    //TODO Should have it's own layer "Area" reffering to a different scene (Area = teleporter)

    //? The teleporter is an Area3D that needs a specific name
    //? previous scene = "PREVIOUS" or "0"
    //? other scenes = name of the Area3D node, for example "level_1" will load "res://scenes/levels/level_1.tscn"

    //TODO END OF SETUP

    private string _previousScene = "";

    private Node _sceneManager = null;
    private int _areaAmount = 0;
    private bool _canSwitch = true;
    private float _switchCooldown = 4.0f;

    public enum TransitionType
    {
        Instant,
        Fade
    }

    public override void _Ready()
    {
        _sceneManager = GetNode("/root/AutoSceneManagerInstance");
        
        GD.Print("\nScene Manager ready.\n");
        RegisterAreas();
    }

    private void RegisterAreas()
    {
        _areaAmount = 0;
        GD.Print("Loading all scenes...");

        foreach (Node areaNode in GetTree().GetNodesInGroup("scene_teleporter_area"))
        {
            if (areaNode is Area3D area)
            {
                area.BodyEntered += (Node3D body) => OnAreaBodyEntered(body, area);

                _areaAmount++;
                GD.Print("Found ", _areaAmount, " areas.");
                GD.Print("-- ", area.Name);
            }
        }
    }

    private async void OnAreaBodyEntered(Node3D body, Area3D teleporter)
    {
        if (!body.IsInGroup("player") || !_canSwitch)
            return;

        _canSwitch = false;
        StartSwitchCooldown();

        string target_scene = "";

        switch (teleporter.Name)
        {
            case "PREVIOUS":
            case "0":
                target_scene = "0";
                break;

            // Add more hardcoded cases here, if needed
            case "EXAMPLE":
                target_scene = "EXAMPLE";
                break;

            default:
                GD.Print("\nScene found!");
                target_scene = VariableManager.SceneRoot + teleporter.Name + ".tscn";
                GD.Print(teleporter.Name, ".tscn");
                break;
        }

        await SwitchScene(target_scene);
    }

    private async void StartSwitchCooldown()
    {
        await ToSignal(GetTree().CreateTimer(_switchCooldown), SceneTreeTimer.SignalName.Timeout);
        _canSwitch = true;
    }

    public async Task SwitchScene(string target_scene, int transition_type = (int)TransitionType.Fade)
    {
        // If the scene switches to "PREVIOUS" or "0", it will go to the previous scene, before updating previous_scene
        if (target_scene == "0")
        {
            if (_previousScene != "")
                target_scene = _previousScene;

            if (_previousScene == "")
            {
                GD.Print("No Previous Scene selected! Returning to world!");
                target_scene = VariableManager.MainScene;
            }
        }

        _previousScene = GetTree().CurrentScene.SceneFilePath;
        GD.Print("Previous Scene: ", _previousScene, "\n");

        switch ((TransitionType)transition_type)
        {
            case TransitionType.Fade:
                _sceneManager.Call("transition_fade", 0);
                await ToSignal(GetTree().CreateTimer(1f), SceneTreeTimer.SignalName.Timeout);

                ChangeScene(target_scene);

                await ToSignal(GetTree().CreateTimer(2f), SceneTreeTimer.SignalName.Timeout);
                _sceneManager.Call("transition_fade", 1);
                break;

            case TransitionType.Instant:
                _sceneManager.Call("transition_instant");
                CallDeferred(nameof(ChangeScene), target_scene);
                break;

            default:
                GD.PrintErr("Unknown transition type: ", transition_type.ToString());
                break;
        }
    }

    private async void ChangeScene(string target_scene)
    {
        GetTree().ChangeSceneToFile(target_scene);

        await ToSignal(GetTree().CreateTimer(2f), SceneTreeTimer.SignalName.Timeout);

        RegisterAreas();

        if (_areaAmount == 0)
        {
            await ToSignal(GetTree().CreateTimer(2f), SceneTreeTimer.SignalName.Timeout);
            GD.Print("something went wrong while getting areas...");
            RegisterAreas();
        }
        else
        {
            GD.Print("New areas active.\n");
        }
    }
}