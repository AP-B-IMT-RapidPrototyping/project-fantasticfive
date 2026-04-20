using Godot;
using System;

public partial class VersionManager : CanvasLayer
{
    //! This script handles version label and SystemDate
    private bool _canKillGame = false;

    [Export] private Control _killUi;
    [Export] private Label _versionLabel;



    public override void _Ready()
    {
        _versionLabel.Text = VariableManager.Version;
        GD.Print(VariableManager.SystemDate.ToString());
        GD.Print(VariableManager.KillDate.ToString());
        GD.Print(VariableManager.DateSeason.ToString());

        if (VariableManager.SystemDate >= VariableManager.KillDate && _canKillGame)
        {
            _killUi.Show();
        }
    }
}
