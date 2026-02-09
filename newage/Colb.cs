using System.Linq;
using Godot;

namespace Adrenalin.newage;

public partial class Colb : Node2D
{
    [Export] PackedScene[] moleculeScenes;
    [Export] private int eachCount;
    int[] createdCounts = new int[7];

    public override void _Ready()
    {
        Timer timer = GetNode<Timer>("Timer");
        timer.Timeout += Timeot;
        timer.Start();
    }

    private RandomNumberGenerator rng = new RandomNumberGenerator();
    void Timeot()
    { 
        if (GetChildren().OfType<Molecule>().Count() > 35) return;
       rng.Randomize();
       Node2D molecule = moleculeScenes[rng.RandiRange(0, moleculeScenes.Length - 1)].Instantiate<Node2D>();
       molecule.Rotation = rng.Randf() * Mathf.Tau;
       molecule.Position = Vector2.FromAngle(molecule.Rotation) * rng.RandfRange(0, 1100);
       AddChild(molecule);
    }
}