using Godot;
using System;
public partial class MenuOptions : Node3D
{
    [Export] private MeshInstance3D _buttonPlay;
    private Color _playDefaultColor;
    [Export] private MeshInstance3D _buttonSettings;
    private Color _settingsDefaultColor;
    [Export] private MeshInstance3D _buttonQuit;
    private Color _quitDefaultColor;
    [Export] private MeshInstance3D _buttonBack;
    private Color _backDefaultColor;
    private Color color;

    [Export] private AnimationPlayer _animCamSwitch;

    [Export] private Control _playOptions;
    [Export] private AudioStreamPlayer _hoverUI;
    [Export] private AudioStreamPlayer _clickUI;

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Visible;

        color = new Color(0.7f, 0.7f, 0.7f);

        // Play button
        var playMaterial = _buttonPlay.GetActiveMaterial(0);
        if (playMaterial is StandardMaterial3D playMat)
        {
            var uniqueMaterial = (StandardMaterial3D)playMat.Duplicate();
            _buttonPlay.SetSurfaceOverrideMaterial(0, uniqueMaterial);
            _playDefaultColor = uniqueMaterial.AlbedoColor;
        }

        // Settings button
        var settingsMaterial = _buttonSettings.GetActiveMaterial(0);
        if (settingsMaterial is StandardMaterial3D settingsMat)
        {
            var uniqueMaterial = (StandardMaterial3D)settingsMat.Duplicate();
            _buttonSettings.SetSurfaceOverrideMaterial(0, uniqueMaterial);
            _settingsDefaultColor = uniqueMaterial.AlbedoColor;
        }

        // Quit button
        var quitMaterial = _buttonQuit.GetActiveMaterial(0);
        if (quitMaterial is StandardMaterial3D quitMat)
        {
            var uniqueMaterial = (StandardMaterial3D)quitMat.Duplicate();
            _buttonQuit.SetSurfaceOverrideMaterial(0, uniqueMaterial);
            _quitDefaultColor = uniqueMaterial.AlbedoColor;
        }

        // Back button
        var backMaterial = _buttonBack.GetActiveMaterial(0);
        if (backMaterial is StandardMaterial3D backMat)
        {
            var uniqueMaterial = (StandardMaterial3D)backMat.Duplicate();
            _buttonBack.SetSurfaceOverrideMaterial(0, uniqueMaterial);
            _backDefaultColor = uniqueMaterial.AlbedoColor;
        }
    }

    /* ------------------------------------------ */
    /* Press Play */
    private void OnPlayPressed(Node camera, InputEvent @event, Vector3 position, Vector3 normal, int shapeIdx)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
        {
            _clickUI.Play();
            _animCamSwitch.Play("play");
        }
    }

    /* Enter Play */
    private void OnPlayEntered()
    {
        _hoverUI.Play();
        var material = _buttonPlay.GetActiveMaterial(0) as StandardMaterial3D;
        if (material != null)
            material.AlbedoColor = new Color(color);
    }

    /* Exit Play */
    private void OnPlayExited()
    {
        var material = _buttonPlay.GetActiveMaterial(0) as StandardMaterial3D;
        if (material != null)
            material.AlbedoColor = _playDefaultColor;
    }

    /* ------------------------------------------ */
    /* Press Settings */
    private void OnSettingsPressed(Node camera, InputEvent @event, Vector3 position, Vector3 normal, int shapeIdx)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
        {
            _clickUI.Play();
            _animCamSwitch.Play("settings");
        }
    }

    /* Enter Settings */
    private void OnSettingsEntered()
    {
        _hoverUI.Play();
        var material = _buttonSettings.GetActiveMaterial(0) as StandardMaterial3D;
        if (material != null)
            material.AlbedoColor = new Color(color);
    }

    /* Exit Settings */
    private void OnSettingsExited()
    {
        var material = _buttonSettings.GetActiveMaterial(0) as StandardMaterial3D;
        if (material != null)
            material.AlbedoColor = _settingsDefaultColor;
    }

    /* ------------------------------------------ */
    /* Press Quit */
    private void OnQuitPressed(Node camera, InputEvent @event, Vector3 position, Vector3 normal, int shapeIdx)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
        {
            _clickUI.Play();
            GetTree().Quit();
        }
    }

    /* Enter Quit */
    private void OnQuitEntered()
    {
        _hoverUI.Play();
        var material = _buttonQuit.GetActiveMaterial(0) as StandardMaterial3D;
        if (material != null)
            material.AlbedoColor = new Color(color);
    }

    /* Exit Quit */
    private void OnQuitExited()
    {
        var material = _buttonQuit.GetActiveMaterial(0) as StandardMaterial3D;
        if (material != null)
            material.AlbedoColor = _quitDefaultColor;
    }

    /* ------------------------------------------ */
    /* Press Back */
    private void OnBackPressed(Node camera, InputEvent @event, Vector3 position, Vector3 normal, int shapeIdx)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
        {
            _clickUI.Play();
            _animCamSwitch.PlayBackwards("settings");
        }
    }

    /* Enter Back */
    private void OnBackEntered()
    {
        _hoverUI.Play();
        var material = _buttonBack.GetActiveMaterial(0) as StandardMaterial3D;
        if (material != null)
            material.AlbedoColor = new Color(color);
    }

    /* Exit Back */
    private void OnBackExited()
    {
        var material = _buttonBack.GetActiveMaterial(0) as StandardMaterial3D;
        if (material != null)
            material.AlbedoColor = _backDefaultColor;
    }
}