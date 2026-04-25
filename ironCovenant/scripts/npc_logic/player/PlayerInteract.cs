using Godot;
using System;
using System.Diagnostics.Tracing;

public partial class PlayerInteract : Node3D
{
    [Export] private InventorySystem _inventory;
    [Export] private RayCast3D _interactRay;
    [Export] private Marker3D _playerHand;

    public bool _isHoldingItem;

    [Export] private ColorRect _crosshair;
    private bool _crosshairIsBig;




    // System
    /*     private void HandleInteractUI()
        {
            if (_interactRay.IsColliding())
            {
                var collider = _interactRay.GetCollider() as Node;

                if (collider.IsInGroup("item") && _crosshair.Scale != new Vector2(20, 20) && !_crosshairIsBig)
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
        } */

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

                if (collider != null && collider.IsInGroup("item") && collider is IInteractable interactableData)
                {
                    GD.Print($"Item: {collider.Name}");
                    bool interactSuccess = _inventory.AddItem(interactableData.Item, interactableData.Amount);

                    if (interactSuccess)
                    {
                        ((Node)interactableData).QueueFree();
                    }
                }
            }
        }
    }

    public void DropItem(ItemData item)
    {
        if (item == null || !_inventory.GetItems().ContainsKey(item)) return;

        if (item != null && !string.IsNullOrEmpty(item.ItemScene))
        {
            var scene = GD.Load<PackedScene>(item.ItemScene);
            var worldItem = scene.Instantiate<Node3D>();

            GetTree().CurrentScene.AddChild(worldItem);
            worldItem.GlobalPosition = _playerHand.GlobalPosition;

            if (worldItem is IInteractable itemScript)
            {
                itemScript.OnDropped();
            }

            _inventory.RemoveItem(item, 1);
        }
        else
        {
            GD.Print($"Item to drop is null or epmty! {item.DisplayName}");
        }
    }

    public void EquipItem(ItemData item)
    {
        GD.Print($"Equipped {item.DisplayName}");
    }




    public override void _PhysicsProcess(double delta)
    {
        Interact();
        /* HandleInteractUI(); */
    }
}
