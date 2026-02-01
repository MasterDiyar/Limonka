using Godot;

[GlobalClass]
public partial class SpawnRes : Resource
{
    [Export] public PackedScene mob;
    [Export] public Vector2 pos;
}