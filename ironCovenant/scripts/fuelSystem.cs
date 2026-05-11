using Godot;
using System;

public partial class fuelSystem : Node
{
    
    [Export] private ItemData fuelItem;
    [Export] private int fuelNeeded = 1;

    public void TryStartTrain()
    {
        if (InventorySystem.Inventory == null)
        {
            GD.Print("Inventory not found");
            return;
        }

        if (InventorySystem.Inventory.HasItem(fuelItem, fuelNeeded))
        {
            GD.Print("Train starts!");

            InventorySystem.Inventory.RemoveItem(fuelItem, fuelNeeded);

            StartTrain();
        }
        else
        {
            GD.Print("Not enough fuel");
        }
    }

    private void StartTrain()
    {
        GD.Print("Train is moving!");
    }
}