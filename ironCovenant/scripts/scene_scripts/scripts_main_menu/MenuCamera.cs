using Godot;

public partial class MenuCamera : Camera3D
{
    [Export] private bool _useCamRotation = true;

    [Export] private float _maxRotationY = 0.05f;
    [Export] private float _maxRotationX = 0.05f;
    [Export] private float _smoothing = 3.0f;

    private Vector3 _baseRotation;
    private bool _allowMouseControl = true;


    public override void _Ready()
    {
        _baseRotation = Rotation;
    }

    public void DisableMouseControl(StringName animation)
    {
        _allowMouseControl = false;
    }
    public void AllowMouseControl(StringName animation)
    {
        _baseRotation = Rotation;
        _allowMouseControl = true;
    }

    public override void _Process(double delta)
    {
        if (!_allowMouseControl || !_useCamRotation)
            return;

        Vector2 mousePos = GetViewport().GetMousePosition();
        Vector2 viewportSize = GetViewport().GetVisibleRect().Size;

        Vector2 normalized = (mousePos / viewportSize) * 2.0f - Vector2.One;

        normalized.Y *= -1;
        normalized.X *= -1;

        float offsetY = normalized.X * _maxRotationY;
        float offsetX = normalized.Y * _maxRotationX;

        Vector3 targetRot = _baseRotation + new Vector3(offsetX, offsetY, 0);

        Rotation = Rotation.Lerp(targetRot, (float)(delta * _smoothing));
    }
}
