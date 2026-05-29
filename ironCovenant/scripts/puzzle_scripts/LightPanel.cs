using Godot;
using System;

public partial class LightPanel : StaticBody3D
{
	[Signal]
	public delegate void GivePowerEventHandler();
	[Signal]
	public delegate void StopPowerEventHandler();

	[Export] protected Timer endPowerTimer;

    public override void _Ready()
    {
        endPowerTimer.Timeout += EmitStopPower;
    }


	virtual public void GetPower()
	{
		GD.Print("Emitting signal: GivePower");
		EmitSignal(SignalName.GivePower);
		
		endPowerTimer.Start();
	}

	protected void EmitStopPower()
	{
		GD.Print("Emitting signal: StopPower");
		EmitSignal(SignalName.StopPower);
	}
}
