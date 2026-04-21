using Godot;
using System;

public partial class PlayerInteract : Node3D
{
    [Export] private ColorRect _crosshair;
    private bool _crosshairIsBig;

    [Export] private RayCast3D _interactRay;

    public bool _isHoldingItem;




    // System
    private void HandleInteractUI()
    {
        if (_interactRay.IsColliding())
        {
            var collider = _interactRay.GetCollider() as Node3D;

            if (collider.IsInGroup("interactable") && _crosshair.Scale != new Vector2(20, 20) && !_crosshairIsBig)
            {
                _crosshair.Scale = new Vector2(20, 20);
                _crosshairIsBig = true;
            }
            else if (_crosshair.Scale == new Vector2(20, 20) && _crosshairIsBig)
            {
                _crosshair.Scale = new Vector2(1, 1);
                _crosshairIsBig = false;
            }
        }
    }

    //! Do we use the player action state machine or just a bool?
    //! Can we interact multiple times? can we drop stuff? how do we drop? D:
    private void Interact()
    {
        if (Input.IsActionJustPressed("interact"))
        {
            if (_interactRay.IsColliding())
            {
                var collider = _interactRay.GetCollider() as Node;

                if (collider != null && collider.IsInGroup("enemy"))
                {
                    GD.Print($"Enemy: {collider.Name}");
                    // do something...
                }

                if (collider != null && collider.IsInGroup("item"))
                {
                    GD.Print($"Item: {collider.Name}");
                    // do something...
                }
            }
        }
    }




    public override void _PhysicsProcess(double delta)
    {
        HandleInteractUI();
        Interact();
    }
}
