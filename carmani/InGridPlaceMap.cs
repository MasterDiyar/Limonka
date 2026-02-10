using Godot;
using System;

public partial class InGridPlaceMap : Node2D
{
	[Export] ChoosePanel[] panels;
	public override void _Ready()
	{
		foreach (var panel in panels)
		{
			panel.GuiInput += (@event) => NewE(@event, panel.ID);
		}
	}

	void NewE(InputEvent @event, int panelID)
	{
		if (@event is InputEventMouseButton { Pressed: true } e)
		{
			CreateSprite(panelID);	
		}
	}

	bool created = false;
	int currentPanelID = 0;
	void CreateSprite(int id)
	{
		created = true;
		currentPanelID = id;
	}

	void AddSprite(int panelID, Vector2 pos)
	{
		
	}
	public override void _Process(double delta)
	{
		if (Input.IsActionJustReleased("lm"))
		{
			Vector2 pos = GetGlobalMousePosition(), pm;
			if (pos.X is > 128f and < 512 && pos.Y is > 128f and < 384)
			{
				pm = new Vector2((int)(pos.X / 64) * 64,(int) (pos.Y/64) * 64);
			}
		}
	}
}
