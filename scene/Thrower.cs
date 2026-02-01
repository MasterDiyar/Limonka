using Godot;
using System;
using Adrenalin.scene;

public partial class Thrower : Node2D
{
    [Export]PackedScene bulletScene;
    [Export] private int count = 1;
    [Export] private float rotation = 0,
                           offsetAngle = 0,
                           offsetPosition = 0,
                           attackTimer = 1;
    private float currentTime = 0;

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (currentTime <= attackTimer ) currentTime += (float)delta;
        
    }

    public bool TryAttack(float angle, float damage, float speed)
    {
        if (!(currentTime >= attackTimer)) return false;
        for (var i = 0; i < count; i++) {
            var bullet = bulletScene.Instantiate<Bullet>();
            bullet.Rotation = angle - offsetAngle + rotation * i;
            bullet.Position = GetGlobalPosition() + Vector2.FromAngle(bullet.Rotation) * offsetPosition;
            bullet.Damage = damage;
            bullet.Speed = speed;
            GetTree().Root.AddChild(bullet);
        }
            
        return true;
    }
}
