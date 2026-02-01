using Godot;

[GlobalClass]
public partial class WorldMapRes : Resource
{
    [Export]
    public float Iron = 0, 
        Energy = 0,
        Food = 0,
        Wood = 0,
        Silver = 0;

    public void AddResource(WorldMapRes res)
    {
        Iron   += res.Iron;
        Energy += res.Energy;
        Food   += res.Food;
        Wood   += res.Wood;
        Silver += res.Silver;
    }
}