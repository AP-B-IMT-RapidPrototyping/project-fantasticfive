using System.Collections.Generic;
using Godot;

public partial class InventorySystem : Node
{
    private Dictionary<string, int> _inventory = new();

    [Export] private int _invMaxSlots = 3;
    [Export] private int _invMaxStack = 10;




    public bool AddItem(string item, int amount)
    {
        if (_inventory.ContainsKey(item))
        {
            if (_inventory[item] + amount > _invMaxStack)
            {
                GD.Print($"Inventory: Can't add {item}. Stack limited reached ({amount}/{_inventory[item]})");
                return false;
            }

            _inventory[item] += amount;
        }
        else
        {
            if (_inventory.Count >= _invMaxSlots)
            {
                GD.Print($"Inventory: Can't add {item}. Slot limited reached ({_inventory.Count}/{_invMaxSlots})");
                return false;
            }

            _inventory.Add(item, amount);
        }

        GD.Print($"Inventory: Added {item} ({amount}). Total: {_inventory[item]}");
        return true;
    }

    public bool RemoveItem(string item, int amount)
    {
        if (!_inventory.ContainsKey(item))
            return false;

        if (_inventory[item] >= amount)
        {
            _inventory[item] -= amount;
            GD.Print($"Inventory: Removed {item} ({amount}). Total: {_inventory[item]}");

            if (_inventory[item] <= 0)
            {
                _inventory.Remove(item);
                GD.Print($"Inventory: Removed {item} (ALL). Total: {_inventory[item]}");
            }
            return true;
        }

        return false;
    }
}