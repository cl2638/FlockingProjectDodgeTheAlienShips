using Godot;
using System;

public partial class HUD : CanvasLayer
{
	private Label _timerLabel;
	private float _elapsedTime = 0f;
	private bool _isRunning = true;

	public override void _Ready()
	{
		_timerLabel = GetNode<Label>("TimerLabel");
		UpdateLabel();
	}

	// Called every frame; updates elapsed time and label if running
	public override void _Process(double delta)
	{
		if (_isRunning)
		{
			_elapsedTime += (float)delta;
			UpdateLabel();
		}
	}

	private void UpdateLabel()
	{
		// Display time in seconds (or format mm:ss if you like)
		_timerLabel.Text = $"Time: {Mathf.FloorToInt(_elapsedTime)}s";
	}

	// Stops the timer from updating
	public void StopTimer()
	{
		_isRunning = false;
	}
}
