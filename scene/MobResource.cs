using Godot;

[GlobalClass]
public partial class MobResource : Resource
{
    [Export] public float MaxHp;
    [Export] public float MaxSpeed;
    [Export] public float MaxDamage;
}