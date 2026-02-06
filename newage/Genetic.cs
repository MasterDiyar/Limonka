using Godot;
using System.Collections.Generic;
using System.Linq;
namespace Adrenalin.newage;

public class Genetic
{
    public Vector2[] ShapePolygon = [];
    public float[] Weights = [];
    public float Area = 0;
    
    RandomNumberGenerator rng = new RandomNumberGenerator();

    Vector2 randVec()
    {
        return new Vector2(rng.RandfRange(0, 100), rng.RandfRange(0, 100));
    }

    float findArea()
    {
        float area = 0;
        Vector2 a, b;
        for (int i = 0; i < ShapePolygon.Length-1; i++)
        {
            a = ShapePolygon[i];
            b = ShapePolygon[i+1];
            area += a.X*b.Y - a.Y*b.X;
        }
        a = ShapePolygon[^1];
        b = ShapePolygon[0];
        area +=  a.X*b.Y - a.Y*b.X;
        return area/2;
    }
    public Genetic(Vector2[] shape = null, float[] weights=null ,bool makeRandom = false)
    {
        if (!makeRandom)
        {
            ShapePolygon = shape;
            Weights = weights;
            Area = findArea();
        }else {
            rng.Randomize();
            ShapePolygon = Enumerable.Range(0, rng.RandiRange(3, 16))
                .Select(_ => randVec())
                .ToArray();
            Weights = new List<float>(rng.RandiRange(3, 6)).Select(flow => rng.RandfRange(0, 2)).ToArray();
        }
    }

    
    
    public override string ToString()
    {
        return base.ToString();
    }
}