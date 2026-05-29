using Godot;
using System;

public partial class FancyLightPanel : LightPanel
{
	[Export] private MeshInstance3D chargeMesh;
	private bool charging = false;
	private bool hasCharged = false;

	private int charge = 0;

	public override void _Ready()
    {
        endPowerTimer.Timeout += StopCharging;
    }

    public override void _Process(double delta)
    {
		

        if (!charging && charge > 0)
		{
			charge--;
			chargeMesh.GlobalPosition -= new Vector3(0, 0.003f, 0);
		}

		if (charge == 0 && hasCharged)
		{
			EmitStopPower();
			hasCharged = false;
		}
    }


    public override void GetPower()
    {
        base.GetPower();
		if (charge < 500)
		{
			charge += 50;
			chargeMesh.GlobalPosition += new Vector3(0, 0.15f, 0);
		}
		charging = true;
		hasCharged = true;
    }


	private void StopCharging()
	{
		charging = false;
	}
}
