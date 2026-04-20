using Godot;
using System;
using System.Linq.Expressions;

public partial class VariableManager : Node
{
    //! Please change the values of the variables if needed
    // ================================== //
    // File Paths
    public static string MainMenuScene = "res://scenes/levels/main_menu.tscn";
    public static string MainScene = "res://scenes/levels/world.tscn";
    public static string SceneRoot = "res://scenes/levels/";



    // ================================== //
    // Game Variables
    public static float Gravity = 12.8f;
    public static float TerminalVelocity = 190f;



    // ================================== //
    // System Versioning
    // Change project version in Project Settings -> Config -> Version
    public static string Version = (string)ProjectSettings.GetSetting("application/config/version");



    // ================================== //
    // System Dates & Seasons
    public static DateTime SystemDate = new DateTime();
    public static DateTime KillDate = new DateTime(); //! SET A KILL DATE HERE
    public static string DateSeason = "None";



    public override void _Ready()
    {
        // ================================== //
        // Update SystemDate
        SystemDate = DateTime.Now;

        DateSeason = (SystemDate.Month, SystemDate.Day) switch
        {
            // Halloween
            (10, 31) => "Halloween",

            // Christmas
            (12, var day) when day >= 20 && day <= 30 => "Christmas",

            _ => "None"
        };



        // ================================== //
        // ...
    }
}
