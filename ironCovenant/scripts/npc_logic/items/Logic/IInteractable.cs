using Godot;
using System;

public interface IInteractable
{
    ItemData Item { get; }
    int Amount { get; set; }

    void OnDropped();
    void OnEquipped();
    void Use();
    void AltUse();
}
