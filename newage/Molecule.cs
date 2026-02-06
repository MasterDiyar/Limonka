using Godot;

namespace Adrenalin.newage;

public partial class Molecule : Area2D 
{
    [Export]public string[] Components;
    [Export] public int[] Count;
    [Export] public string UpperName;
    
}