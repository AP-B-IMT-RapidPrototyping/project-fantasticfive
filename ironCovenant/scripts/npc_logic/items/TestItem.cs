using Godot;
using System;

public partial class TestItem : RigidBody3D, IInteractable
{
    [Export] public ItemData Item {get; set;}
    public int Amount {get; set;} = 1;




public void OnDropped()
    {
        Freeze = false;
    }

    public void OnEquipped()
    {
        Freeze = true;
    }


    public void Action()
    {
        GD.Print($"{Item.DisplayName} done.");
    }
}
