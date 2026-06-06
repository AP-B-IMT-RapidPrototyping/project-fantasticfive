using Godot;
using System;

public partial class fuelSystem : Node
{
    
    [Export] private ItemData fuelItem;
    [Export] private ItemData teddyItem;
    [Export] private Label suggestionLabel;
    [Export] private Timer suggestionTimer;
    [Export] private int fuelNeeded = 1;

    [Export] private AnimationPlayer trainAnim;
    [Export] private AnimationPlayer doorAnim;

    public int currentLevel = 1;

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
        } else  if (InventorySystem.Inventory.HasItem(teddyItem, 1))
        {
            GD.Print("Train starts!");

            InventorySystem.Inventory.RemoveItem(teddyItem, 1);

            StartTrain();
        }
        else
        {
            if (currentLevel == 1)
            {
                suggestionLabel.Text = "The train doesn't want anything rn.";
            } else if (currentLevel == 2)
            {
                GD.Print("Not enough fuel");
                suggestionLabel.Text = "The train needs fuel to start.";
            } else if (currentLevel == 3)
            {
                GD.Print("Not enough teddy");
                suggestionLabel.Text = "The train needs a teddy bear to start.";
            } else if (currentLevel == 4)
            {
                suggestionLabel.Text = "The train senses danger.";
            }
            
            suggestionTimer.Start();
        }
    }

    private void StartTrain()
    {
        GD.Print("Train is moving!");
        trainAnim.Play("play");
        doorAnim.Play("play");
    }

    public void ClearLabel()
    {
        suggestionLabel.Text = "";
    }
}