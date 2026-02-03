using Godot;
using System;
using Adrenalin.scene;

public partial class MovingUnit : Mob
{
	
	public override void _Ready()
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		var norm = Input.GetVector("a", "d", "w", "s");
		var nirm = norm.Normalized();
		
		Velocity += nirm * 100*(float)delta;
		MoveAndSlide();
	}
}
