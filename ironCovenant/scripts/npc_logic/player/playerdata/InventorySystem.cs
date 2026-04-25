using System.Collections.Generic;
using Godot;

public partial class InventorySystem : Node
{
    [Signal] public delegate void InventoryUpdatedEventHandler();

    private Dictionary<ItemData, int> _inventory = new();
    public Dictionary<ItemData, int> GetItems() => _inventory;

    [Export] private int _invMaxSlots = 6;
    [Export] private int _invMaxStack = 10;




    public bool AddItem(ItemData item, int amount = 1)
    {
        if (_inventory.ContainsKey(item))
        {
            if (_inventory[item] + amount > _invMaxStack)
            {
                GD.Print($"Inventory: Can't add {item.DisplayName}. Stack limited reached ({amount}/{_inventory[item]})");
                return false;
            }

            _inventory[item] += amount;
        }
        else
        {
            if (_inventory.Count >= _invMaxSlots)
            {
                GD.Print($"Inventory: Can't add {item.DisplayName}. Slot limited reached ({_inventory.Count}/{_invMaxSlots})");
                return false;
            }

            _inventory.Add(item, amount);
        }

        GD.Print($"Inventory: Added {item.DisplayName} ({amount}). Total: {_inventory[item]}");
        EmitSignal(SignalName.InventoryUpdated);
        return true;
    }

    public ItemData RemoveItem(ItemData item, int amount = 1)
    {
        if (item == null || !_inventory.ContainsKey(item)) return null;

        if (_inventory[item] >= amount)
        {
            _inventory[item] -= amount;

            GD.Print($"Inventory: Removed {item.DisplayName} ({amount}). Total: {_inventory[item]}");

            if (_inventory[item] <= 0)
            {
                GD.Print($"Inventory: Removed {item.DisplayName} (ALL). Total: {_inventory[item]}");
                _inventory.Remove(item);
            }

            EmitSignal(SignalName.InventoryUpdated);
            return item;
        }

        return null;
    }
}