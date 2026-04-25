using Godot;
using System;
using System.ComponentModel.DataAnnotations;

public partial class InventoryUi : Control
{
    [Export] private PlayerHead _playerHead;
    [Export] private PlayerInteract _playerInteract;

    [Export] private InventorySystem _inventory;
    [Export] private VBoxContainer _itemContainer; // location
    [Export] private PackedScene _itemSlot;
    private ItemData _selectedItem = null;

    [Export] private AnimationPlayer _inventoryAnim;
    private bool _isInvActive = false;




    public override void _Ready()
    {
        _inventory.InventoryUpdated += () => CallDeferred(nameof(UpdateInventoryUI));
    }


    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("inventory"))
        {
            if (!_isInvActive)
            {
                _playerHead._cameraLocked = true;
                _inventoryAnim.Play("show");
                Input.MouseMode = Input.MouseModeEnum.Visible;
            }
            else
            {
                Input.MouseMode = Input.MouseModeEnum.Captured;
                _playerHead._cameraLocked = false;
                _inventoryAnim.PlayBackwards("show");
            }

            _isInvActive = !_isInvActive;
        }
    }


    private void UpdateInventoryUI() // removes current items and gets items from inventory and adds as child
    {
        foreach (Node child in _itemContainer.GetChildren())
        {
            child.QueueFree();
        }

        foreach (var item in _inventory.GetItems())
        {
            ItemData data = item.Key;
            int amount = item.Value;

            var slot = _itemSlot.Instantiate<InventorySlot>();
            _itemContainer.AddChild(slot);

            slot.Create(data, amount, () =>
            {
                _playerInteract.DropItem(data);
                GD.Print("DROPPED");
            },
            () =>
            {
                _playerInteract.EquipItem(data);
                GD.Print("EQUIPPED!");
            });
        }
    }
}
