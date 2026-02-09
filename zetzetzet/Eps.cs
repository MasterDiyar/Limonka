using Godot;
using System;

public partial class Eps : CharacterBody2D
{
    [Export] public RayCast2D RayLeft, RayRight, RayForward;
    
    public float Fitness = 0, prevDist = 0;
    public Vector2 TargetPos;
    public float Speed = 50f;
    
    public float[] WeightsIn = new float[20]; 
    public float[] WeightsOut = new float[8];
    
    public float[] BiasHidden = new float[4];
    public float[] BiasOutput = new float[2];

    float[] GetInputs()
    {
        float angleToTarget =
                 Mathf.Wrap(
                     (TargetPos - GlobalPosition).Angle() - Rotation,
                     -Mathf.Pi,
                     Mathf.Pi
                 ) / Mathf.Pi;
        
        return [GetDistance(RayLeft), GetDistance(RayForward) ,GetDistance(RayRight), angleToTarget, Velocity.Length() / Speed];
    }
    public override void _PhysicsProcess(double delta)
    {
        float[] inputs = GetInputs(); 
        float[] hidden = new float[4];

        for (int h = 0; h < 4; h++) {
            for (int i = 0; i < 5; i++)
                hidden[h] += inputs[i] * WeightsIn[h * 3 + i];
            hidden[h] = MathF.Tanh(hidden[h]+ BiasHidden[h]); 
        }

        float turn = BiasOutput[0];
        float acceleration = BiasOutput[1];
        for (int h = 0; h < 4; h++)
        {
            turn += hidden[h] * WeightsOut[h * 2];
            acceleration += hidden[h] * WeightsOut[h * 2 + 1];
        }

        Rotation += turn * (float)delta * 5f;
        acceleration = MathF.Tanh(acceleration);
        Velocity = Transform.X * ((acceleration + 1f) * 0.5f) * Speed;

        var collision = MoveAndSlide();
        if (GetSlideCollisionCount() > 0)
        {
            Fitness -= 10f; 
        }
        
        float currentDist = GlobalPosition.DistanceTo(TargetPos);
        Fitness += (prevDist - currentDist) * 10f;
        prevDist = currentDist;
        
        if (Velocity.Length() < 5)
            Fitness -= 20 * (float)delta;
        Fitness -= 0.01f; 

        if (currentDist < 10f)
            Fitness += 1000f;
    }

    private float GetDistance(RayCast2D ray)
    {
        if (ray.IsColliding())
            return (ray.GetCollisionPoint() - GlobalPosition).Length() / 200f; 
        return 1.0f;
    }

    public void SetWeights(float[] wIn, float[] wOut, float[] bHid, float[] bOut)
    {
        WeightsIn = (float[])wIn.Clone();
        WeightsOut = (float[])wOut.Clone();
        BiasHidden = (float[])bHid.Clone();
        BiasOutput = (float[])bOut.Clone();
    }
}