using Godot;
using System;
using Adrenalin.scene;

public partial class Player : Mob
{
    [Export] public float MaxShield;
    [Export] public Label oreInfo;
    [Export] public Button[] buyButtons;
    private Map mao;
    public float Shield;

    public override void _Ready()
    {
        base._Ready();
        mao    = GetParent<Map>();
        Shield = MaxShield;
        mao.ResourceAdded += chageLabel;
        buyButtons[0].Pressed += AddTurret;
        chageLabel();
    }

    public void chageLabel()
    {
        oreInfo.Text = $"Iron: {mao.res.Iron}\nEnergy: {mao.res.Energy}\n" +
                       $"Food: {mao.res.Food}\nWood: {mao.res.Wood}\nSilver: {mao.res.Silver}";
    }

    public void AddTurret()
    {
        
        var turret = Map.TurretScenes[0].Instantiate();
        AddChild(turret);
    }

    public override void TakeDamage(float damage)
    {
        Hp -= damage/Shield;
    }
    
    public void Update(MobResource stat)
    {
        Upgradeable.MaxHp = stat.MaxHp;
        Upgradeable.MaxSpeed = stat.MaxSpeed;
        Upgradeable.MaxDamage = stat.MaxDamage;
    }
    
    
}
