using Godot;
using System;
using System.Security.Cryptography.X509Certificates;

public partial class InventorySlot : Control
{
    [Export] private Label _itemName;
    [Export] private Button _dropBtn;
    [Export] private Button _equipBtn;

    public void Create(ItemData data, int amount, System.Action onDrop, System.Action onEquip)
    {
        _itemName.Text = $"{data.DisplayName}: {amount}";

        _dropBtn.Pressed += () => onDrop?.Invoke();
        _equipBtn.Pressed += () => onEquip?.Invoke();
    }
}
