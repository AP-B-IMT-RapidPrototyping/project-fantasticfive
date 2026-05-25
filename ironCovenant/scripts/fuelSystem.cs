using Godot;
using System;

public partial class fuelSystem : Node
{
    
    [Export] private ItemData fuelItem;
    [Export] private Label suggestionLabel;
    [Export] private Timer suggestionTimer;
    [Export] private int fuelNeeded = 1;

    public override void _Ready()
    {
        suggestionTimer.Timeout += ClearLabel;
    }


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
            suggestionLabel.Text = "The train needs fuel to start.";
            suggestionTimer.Start();
        }
    }

    private void StartTrain()
    {
        GD.Print("Train is moving!");
    }

    public void ClearLabel()
    {
        suggestionLabel.Text = "";
    }
}