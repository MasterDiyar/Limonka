using Godot;
using System;
using Adrenalin.newage;

public partial class Microb : Node2D
{
    private Genetic genes;
    public override void _Ready()
    {
        genes = new Genetic();
        
    }
}
