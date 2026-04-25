using Godot;
using System;
using System.Diagnostics.Tracing;

public partial class PlayerInteract : Node3D
{
    [Export] private InventorySystem _inventory;
    [Export] private RayCast3D _interactRay;
    [Export] private Marker3D _playerHand;

    [Export] private ColorRect _crosshair;
    private bool _crosshairIsBig;

    private Node3D _heldItemNode = null;
    private ItemData _heldItemData = null;




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
        if (_heldItemNode != null && _heldItemData == item)
        {
            _heldItemNode.QueueFree();
            _heldItemNode = null;
            _heldItemData = null;
        }

        if (item == null || !_inventory.GetItems().ContainsKey(item)) return;

        if (item != null && !string.IsNullOrEmpty(item.ItemScene))
        {
            var scene = GD.Load<PackedScene>(item.ItemScene);
            var worldItem = scene.Instantiate<Node3D>();

            // spawn the dropped item
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
        if (_heldItemNode != null)
        {
            _heldItemNode.QueueFree();
        }

        if (item != null && !string.IsNullOrEmpty(item.ItemScene))
        {
            var scene = GD.Load<PackedScene>(item.ItemScene);
            _heldItemNode = scene.Instantiate<Node3D>();
            _heldItemData = item;

            _playerHand.AddChild(_heldItemNode);

            if (_heldItemNode is IInteractable interactable)
            {
                interactable.OnEquipped();
            }
            GD.Print($"Equipped {item.DisplayName}");
        }
        else
        {
            GD.Print($"Item to equip is null or empty! {item.DisplayName}");
        }
    }



    // No unhandled input. i know, maybe call funcs from elsewhere?
    private void OnUsePressed()
    {
        if (Input.IsActionJustPressed("mouse1"))
        {
            if (_heldItemNode is IInteractable interactable)
            {
                interactable.Use();
            }
        }

        if (Input.IsActionJustPressed("mouse2"))
        {
            if (_heldItemNode is IInteractable interactable)
            {
                interactable.AltUse();
            }
        }
    }

    private void OnDropPressed()
    {
        if (Input.IsActionJustPressed("drop"))
        {
            if (_heldItemData != null)
            {
                GD.Print($"Dropped equipped item {_heldItemData.DisplayName}");
                DropItem(_heldItemData);
            }
        }
    }




    public override void _PhysicsProcess(double delta)
    {
        Interact();
        /* HandleInteractUI(); */

        if (_heldItemNode == null) return;
        OnUsePressed();
        OnDropPressed();
    }
}
