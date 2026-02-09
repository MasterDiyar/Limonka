using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Mapa : Node2D
{
    [Export] public PackedScene EpsScene;
    [Export] public int PopulationSize = 100;
    [Export] public float GenerationTime = 25f;

    private int generationCount = 1; 
    private List<Eps> currentPopulation = new List<Eps>();
    
    private float[] bestWeightsIn;
    private float[] bestWeightsOut;
    private float[] bestBiasIn;
    private float[] bestBiasOut;

    private Vector2 spawnPos = new Vector2(0, 0);
    private Vector2 goalPos = new Vector2(535, 250);

    public override void _Ready()
    {
        rng.Randomize();
        bestWeightsIn = GenerateRandomWeights(20); 
        bestWeightsOut = GenerateRandomWeights(8); 
        bestBiasIn = GenerateRandomWeights(4);
        bestBiasOut = GenerateRandomWeights(2);
        StartGeneration();
    }

    async void StartGeneration()
    {
        GD.Print($"--- ПОКОЛЕНИЕ {generationCount} ---");
        
        foreach (var bot in currentPopulation) bot.QueueFree();
        currentPopulation.Clear();

        for (int i = 0; i < PopulationSize; i++)
        {
            var bot = EpsScene.Instantiate<Eps>();
            AddChild(bot);
            bot.GlobalPosition = spawnPos;
            bot.TargetPos = goalPos;

            if (generationCount == 1) {
                bot.SetWeights(GenerateRandomWeights(20), GenerateRandomWeights(8), GenerateRandomWeights(4), GenerateRandomWeights(2));
            } else {
                if (i == 0) bot.SetWeights(bestWeightsIn, bestWeightsOut, bestBiasIn, bestBiasOut);
                else bot.SetWeights(Mutate(bestWeightsIn), Mutate(bestWeightsOut), Mutate(bestBiasIn), Mutate(bestBiasOut));
            }

            currentPopulation.Add(bot);
        }

        await ToSignal(GetTree().CreateTimer(GenerationTime), "timeout");

        var winner = currentPopulation.OrderByDescending(b => b.Fitness).First();
        bestWeightsIn = (float[])winner.WeightsIn.Clone();
        bestWeightsOut = (float[])winner.WeightsOut.Clone();
        bestBiasIn = (float[])winner.BiasHidden.Clone();
        bestBiasOut = (float[])winner.BiasOutput.Clone();

        GD.Print($"Поколение {generationCount} завершено. Лучший фитнес: {winner.Fitness} Vel {winner.Velocity}");
        
        generationCount++; 
        StartGeneration();
    }

    float[] Mutate(float[] parent)
    {
        float[] newWeights = (float[])parent.Clone();
        float mutationRate = generationCount < 20 ? 0.1f : 0.03f;
        for (int i = 0; i < newWeights.Length; i++)
        {
            if (rng.Randf() < mutationRate) 
                newWeights[i] += rng.RandfRange(-0.3f, 0.3f);
            
        }
        return newWeights;
    }
    RandomNumberGenerator rng = new RandomNumberGenerator();
    float[] GenerateRandomWeights(int size)
    {
        float[] w = new float[size];
        for (int i = 0; i < size; i++) w[i] = rng.RandfRange(-1.0f, 1.0f);
        return w;
    }
    
    [Export] public Line2D LeaderLine; // Перетащите узел Line2D сюда в инспекторе

    public override void _Process(double delta)
    {
        DrawLineToBestBot();
    }

    private void DrawLineToBestBot()
    {
        if (currentPopulation.Count == 0 || LeaderLine == null) return;

        Eps closestBot = null;
        float minDistance = float.MinValue;
    
        foreach (var bot in currentPopulation)
        {
            if (IsInstanceValid(bot)) {
                if (bot.Fitness > minDistance)
                {
                    minDistance = bot.Fitness;
                    closestBot = bot;
                }
            }
        }

        if (closestBot != null)
        {
            LeaderLine.ClearPoints();
            LeaderLine.AddPoint(goalPos);
            LeaderLine.AddPoint(closestBot.GlobalPosition);
        }
    }
}