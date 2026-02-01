using Godot;

namespace Adrenalin.scene;

public partial class Mob : CharacterBody2D
{
    [ExportGroup("MobInfo")] 
    [Export] protected MobResource StatInfo;
    [Export] public    MobResource Upgradeable;
    public float Hp;
    public float Speed;

    public override void _Ready()
    {
        Hp = StatInfo.MaxHp + Upgradeable.MaxHp;
    }

    public virtual void TakeDamage(float damage)
    {
        Hp -= damage;
        
        if (Hp <= 0) QueueFree();
    }
}