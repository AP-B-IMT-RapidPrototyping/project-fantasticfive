using Godot;
using System;

[GlobalClass]
public partial class ItemData : Resource
{
    [Export] public string ItemID;
    [Export] public string DisplayName;
    [Export] public bool IsDroppable = true;

    [Export(PropertyHint.File, "*.tscn")] public string ItemScene;
}
