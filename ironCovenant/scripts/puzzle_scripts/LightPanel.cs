using Godot;
using System;

public partial class LightPanel : StaticBody3D
{
	[Signal]
	public delegate void GivePowerEventHandler();
	[Signal]
	public delegate void StopPowerEventHandler();

	[Export] private Timer endPowerTimer;

    public override void _Ready()
    {
        endPowerTimer.Timeout += EmitStopPower;
    }


	public void GetPower()
	{
		//GD.Print("Emitting signal: GivePower");
		EmitSignal(SignalName.GivePower);
		
		endPowerTimer.Start();
	}

	private void EmitStopPower()
	{
		//GD.Print("Emitting signal: StopPower");
		EmitSignal(SignalName.StopPower);
	}
}
