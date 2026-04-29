using Godot;
using System;

public partial class Gun : Node // change node voor welke node het is
{
    [Export] int fireRate = 1;
    [Export] int damage = 10;
    [Export] RayCast3D aimRay;

    
}
