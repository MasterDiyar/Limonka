using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adrenalin.scene;

public partial class Dungeon : Node2D
{
    [Export] private int Count = 5;
    [Export] private Godot.Collections.Dictionary<int, PackedScene> scene;
    private Mob cam;
    private int[,] rooms = new int[32, 32];
    private RandomNumberGenerator rng = new RandomNumberGenerator();
    
    static Vector2I vec(int i, int j) => new Vector2I(i, j);
    
    public override void _Ready()
    {
        cam = GetNode<Mob>("movingUnit");
        cam.Position = vec(8192, 8192);
        DungeonInit();
        
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton eventMouseMotion)
        {
            if (eventMouseMotion.Pressed)
                cam.Position = GetGlobalMousePosition();
        }
    }

    void DungeonInit()
    {
        Vector2I startPos = vec(16, 16);
        int firstDir = GetOpposite((new int[] { 1, 2, 4, 8 })[rng.RandiRange(0, 3)]);
        
        rooms[startPos.X, startPos.Y] = firstDir;
        
        CreateNumeredDungeon(startPos + GetDirV(firstDir), firstDir, Count);
        

        StringBuilder sb = new StringBuilder("\n--- Dungeon Map ---\n");
        for (int j = 0; j < 32; j++) {
            for (int i = 0; i < 32; i++) {
                int val = rooms[i, j];
                string cell = val == 0 ? " . " : val.ToString().PadLeft(2) + " ";
                sb.Append(cell);
                if (val ==0) continue;
                CreateCurr(scene[val], vec(i * 544, j * 544));
                
                if ((val & 4) != 0) 
                    CreateTunnel(1, vec(i * 544, j * 544) + Vector2.Right * 272);
                
                if ((val & 8) != 0) 
                    CreateTunnel(2, vec(i * 544, j * 544) + Vector2.Down * 272);
                
            }
            sb.AppendLine();
        }
        GD.Print(sb.ToString());
        
        QueueRedraw();
    }
    
    public override void _Draw()
    {
        float cellSize = 10f; 
        for (int i = 0; i < 32; i++)
            for (int j = 0; j < 32; j++)
                if (rooms[i, j] > 0)
                    DrawRect(new Rect2(i * cellSize, j * cellSize, cellSize, cellSize), Colors.Green);
    }

    void CreateNumeredDungeon(Vector2I current, int from, int left)
    {
        if (current.X < 0 || current.X >= 32 || current.Y < 0 || current.Y >= 32) return;
        if (rooms[current.X, current.Y] != 0) return;
        
        int currentMask = from;
        
        if (left <= 0) {
            rooms[current.X, current.Y] = FlipMask(currentMask);
            return;
        }

        var available = new List<int> { 1, 2, 4, 8 };
        available.Remove(from);

        available = available.Where(d => {
            Vector2I next = current + GetDirV(d);
            return next.X is >= 0 and < 32 && next.Y is >= 0 and < 32 && rooms[next.X, next.Y] == 0;
        }).ToList();

        if (available.Count == 0)
        {
            rooms[current.X, current.Y] = FlipMask(currentMask);
            return;
        }

        int branchCount = rng.RandiRange(1, Math.Min(available.Count, left));
        List<int> chosenDirs = new List<int>();

        for (int i = 0; i < branchCount; i++)
        {
            int idx = rng.RandiRange(0, available.Count - 1);
            int dir = available[idx];
            chosenDirs.Add(dir);
            currentMask += dir; 
            available.RemoveAt(idx);
        }

        rooms[current.X, current.Y] = FlipMask(currentMask);

        int leftPerBranch = left - branchCount;
        foreach (int dir in chosenDirs)
        {
            int roomsForThisBranch = branchCount > 1 ? leftPerBranch / branchCount : leftPerBranch;
            CreateNumeredDungeon(current + GetDirV(dir), GetOpposite(dir), roomsForThisBranch);
        }
    }
    
    void CreateTunnel(int from, Vector2 pos)
    {
        var kletka = (from is 1 or 4 ? scene[20] : scene[21]).Instantiate<Node2D>();
        kletka.Position = pos;
        AddChild(kletka);
    }
    
    Node2D CreateCurr(PackedScene scena, Vector2 position)
    {
        var kletka = scena.Instantiate<Node2D>();
        kletka.Position = position;
        AddChild(kletka);
        return kletka;
    }
    
    int GetOpposite(int dir) => dir switch
        { 1 => 4, 2 => 8, 4 => 1, 8 => 2, _ => 0 };
    
    int FlipMask(int val) => ((val & 1) << 2) | ((val & 4) >> 2) | ((val & 2) << 2) | ((val & 8) >> 2);

    Vector2I GetDirV(int dir) => dir switch
        { 1 => Vector2I.Right, 2 => Vector2I.Down, 4 => Vector2I.Left, 8 => Vector2I.Up, _ => Vector2I.Zero };
    
    Vector2 GetDir(int dir) => GetDirV(dir);
}