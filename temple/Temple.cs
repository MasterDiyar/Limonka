using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Temple : Node2D
{
    [Export] private int Сount = 5;
    [Export] private Godot.Collections.Dictionary<int, PackedScene> scene;
    RandomNumberGenerator rng = new RandomNumberGenerator();
    private float Multiplier = 272;
    private Camera2D cam;
    private int[,] rooms = new int[32, 32];
    
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
       int startX = 16, startY = 16;
       rooms[startX, startY] = 1;
       int bm = (new int[] { 1, 2, 4, 8 })[rng.RandiRange(0, 3)];
       var a = CreateCurr(this, scene[bm], Vector2.One * 512);
       
       LoadTemples(a, bm, Сount, vec(startX, startY), true);
    }

    public void LoadTemples(Node2D prev, int from, int left, Vector2I prevPos, bool isFirst = false)
    {
       Vector2I offsetI = GetDirV(from);
       Vector2I currentPos = prevPos + offsetI;
       
       if (rooms[currentPos.X, currentPos.Y] == 1 && !isFirst) return;
       rooms[currentPos.X, currentPos.Y] = 1;

       Vector2 offset = (Vector2)offsetI * -Multiplier;
       int to = from switch { 1 => 4, 2 => 8, 4 => 1, 8 => 2 };

       if (left <= 0) {
          CreateTunnel(prev, to, offset);
          CreateCurr(prev, scene[to], offset * 2);
          return;
       }

       var avalible = new List<int> { 1, 2, 4, 8 };
       avalible.Remove(to);
       
       avalible = avalible.Where(d => {
          Vector2I nextV = GetDirV(d);
          return rooms[currentPos.X + nextV.X, currentPos.Y + nextV.Y] == 0;
       }).ToList();

       if (avalible.Count == 0) {
          CreateTunnel(prev, to, offset);
          CreateCurr(prev, scene[to], offset * 2);
          return;
       }

       int count = isFirst ? 1 : rng.RandiRange(1, Math.Min(avalible.Count, left));
       
       int a = 0, b = 0, c = 0;
       a = avalible[rng.RandiRange(0, avalible.Count - 1)];
       avalible.Remove(a);
       if (count > 1) { b = avalible[rng.RandiRange(0, avalible.Count - 1)]; avalible.Remove(b); }
       if (count > 2) { c = avalible[0]; }

       left -= count;
       Node2D bn;

       int mask = to + a + b + c;
       bn = CreateCurr(prev, scene[mask], offset * 2);

       switch (count)
       {
          case 1:
             CreateTunnel(bn, a, GetDir(a) * Multiplier);
             LoadTemples(bn, a, left, currentPos); 
             break;
          case 2:
             CreateTunnel(bn, a, GetDir(a) * Multiplier);
             LoadTemples(bn, a, left / 2, currentPos);
             CreateTunnel(bn, b, GetDir(b) * Multiplier);
             LoadTemples(bn, b, left - (left / 2), currentPos);
             break;
          case 3:
             CreateTunnel(bn, a, GetDir(a) * Multiplier);
             LoadTemples(bn, a, left / 3, currentPos);
             CreateTunnel(bn, b, GetDir(b) * Multiplier);
             LoadTemples(bn, b, (left - (left / 3)) / 2, currentPos);
             CreateTunnel(bn, c, GetDir(c) * Multiplier);
             LoadTemples(bn, c, left - (left / 3) - ((left - (left / 3)) / 2), currentPos);
             break;
       }
    }

    Vector2I GetDirV(int dir) => dir switch
       { 1 => Vector2I.Right, 2 => Vector2I.Down, 4 => Vector2I.Left, 8 => Vector2I.Up, _ => Vector2I.Zero };
    Vector2 GetDir(int dir) => GetDirV(dir);
    
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