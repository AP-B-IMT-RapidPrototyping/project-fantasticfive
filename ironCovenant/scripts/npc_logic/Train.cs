using Godot;
using System;

public partial class Train : AnimatableBody3D
{
	[Export] private AnimationPlayer _chestAnim;
	[Export] private AnimationPlayer _doorAnim;
	public bool _canCloseDoors = false;
	private bool _doorAlreadyClosed = false;
	public bool _playerInTrain;




	private void OnDoorBodyEntered(Node3D body)
	{
		if (_canCloseDoors && !_doorAlreadyClosed)
		{
			_doorAnim.Play("play");
			_doorAlreadyClosed = true;
		}

		_playerInTrain = !_playerInTrain;
	}	


	public void OpenBox()
	{
		_chestAnim.Play("openBox");
	}

	public void CloseBox()
	{
		_chestAnim.Play("closeBox");
	}
}
