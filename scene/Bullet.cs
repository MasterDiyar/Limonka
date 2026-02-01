using Godot;
using System;
namespace Adrenalin.scene;

public partial class Bullet : Area2D
{
    public Mob Alpha;
    [Export] public float Damage = 10;
    [Export] public float Speed = 20;
    [Export] public float LifeTime = 2;
    [Export] public float ConsumeOnCollide = 1;

    public override void _Ready()
    {
        BodyEntered += OnAreaEntered;
    }

    public override void _Process(double delta)
    {
        Position += Speed * (float)delta * Vector2.FromAngle(Rotation);
        if (LifeTime > 0) LifeTime-=(float)delta;
        else QueueFree();
    }

    private void OnAreaEntered(Node2D area)
    {
        if (area is not Mob mob || area == Alpha) return;
        
        mob.TakeDamage(Damage);
        LifeTime -= ConsumeOnCollide;
    }
}
