using Godot;
using System;

public partial class Map : Node2D
{
    [Export]
    public WorldMapRes res;

    public Action ResourceAdded;

    public static PackedScene[] TurretScenes = [
        GD.Load<PackedScene>("res://scene/thrower.tscn")
    ]; 

    public void AddResource(WorldMapRes res)
    {
        res.AddResource(res);
        ResourceAdded?.Invoke();
    }


}
