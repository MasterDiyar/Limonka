using Godot;
using System;
using System.Collections.Generic;

public partial class Temple : Node2D
{
	//	  2
	//  1   4  
	//    8
	[Export] private int Сount = 5;
	[Export] private Godot.Collections.Dictionary<int, PackedScene> scene;
	RandomNumberGenerator rng = new RandomNumberGenerator();
	private float Multiplier = 272;
	private Camera2D cam;
	private int[,] rooms = new int[16, 16];
	
	static Vector2I vec(int i, int j) => new Vector2I(i, j);
	
	public override void _Ready()
	{
		cam = GetNode<Camera2D>("Camera2D");
		
		TempleInit();
		
	}


	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton eventMouseMotion)
		{
			if (eventMouseMotion.Pressed)
				cam.Position = GetGlobalMousePosition();
		}
	}

	void TempleInit()
	{
		rooms[8, 8] = 1;
		int bm = (new int[] {1, 2, 4, 8 })[rng.RandiRange(0, 3)];
        		var a = CreateCurr(this, scene[bm], Vector2.One * 512);
        		LoadTemples(a, bm, Сount, vec(8, 8));
	}

	public void LoadTemples(Node2D prev, int from, int left, Vector2I prevPos)
	{
		int to = 0;
		Vector2 offset = from switch {
			1 => Vector2.Right,
			2 => Vector2.Down,
			4 => Vector2.Left,
			8 => Vector2.Up
		} * -272;
										//В планах испавить код и добавить через Vector2I проверку ыыы
		to = from switch {
        				1 => 4,
        				2 => 8,
        				4 => 1,
        				8 => 2
        			};
		if (left == 0) {
			CreateTunnel(prev, to, offset);
			CreateCurr(prev, scene[to], offset*2);
		} else {
			var count = rng.RandiRange(1, Math.Min(3, left));
			List<int> avalible = [1, 2, 4, 8];
			avalible.Remove(to);
			int a = avalible[rng.RandiRange(0, 2)];
			avalible.Remove(a);
			int b = avalible[rng.RandiRange(0, 1)];
			avalible.Remove(b);
			int c = avalible[0];
			left -= count;
			Node2D bn;
			switch (count)
			{
				case 1: 
					bn =CreateCurr(prev, scene[to+a], offset*2);
					CreateTunnel(prev, a, offset);
					LoadTemples(bn, a, left); break;
				case 2:
					bn = CreateCurr(prev, scene[to+a+b], offset*2);
					CreateTunnel(prev, a, offset);
					LoadTemples(bn, a, left/2);
					left -= left / 2;
					CreateTunnel(prev, b, offset);
					LoadTemples(bn, b, left);
					break;
				case 3:
					bn = CreateCurr(prev, scene[15], offset*2);
					CreateTunnel(prev, a, offset);
					LoadTemples(bn, a, left/3); 
					left -= left / 3;
					CreateTunnel(prev, b, offset);
					LoadTemples(bn, b, left/2);
					left -= left / 2;
					CreateTunnel(prev, c, offset);
					LoadTemples(bn, c, left);
					break;
			}
		}
		 
	}

	void CreateTunnel(Node whom, int from, Vector2 pos)
	{
		var kletka = (from is 1 or 4 ? scene[20] : scene[21]).Instantiate<Node2D>();
		kletka.Position = pos;
		whom.AddChild(kletka);
	}
	
	Node2D CreateCurr(Node whom, PackedScene scena, Vector2 position)
	{
		var kletka = scena.Instantiate<Node2D>();
		kletka.Position = position;
		whom.AddChild(kletka);
		return kletka;
	}
}
