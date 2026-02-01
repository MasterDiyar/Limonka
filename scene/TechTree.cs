using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class TechTree : Control
{
    [Export] private Tekshe bas;
    [Export] private float ratio = 60;
    [Export] private HSlider hSlider;
 
    public override void _Ready()
    {
        hSlider.DragEnded += (a) => {
            ratio = (float)hSlider.Value;
            if (a) {
                GetTotalTekshe(bas);
                QueueRedraw();
            }
        };
        GetTotalTekshe(bas);
    }

    public override void _Draw()
    {
        if (bas != null)
        {
            DrawConnections(bas);
        }
    }

    private void DrawConnections(Tekshe parent)
    {
        foreach (var child in parent.GetChildren())
        {
            if (child is Tekshe childTekshe)
            {
                Vector2 from = parent.GlobalPosition - GlobalPosition + parent.Size / 2;
                Vector2 to = childTekshe.GlobalPosition - GlobalPosition + childTekshe.Size / 2;

                DrawLine(from, to, new Color(1, 1, 1, 0.5f), 2.0f, true);
            
                DrawConnections(childTekshe);
            }
        }
    }

    float GetTotalTekshe(Tekshe tekshe)
    {
        var child = new List<Tekshe>();
        float i = 1;
        foreach (var chil in tekshe.GetChildren()) {
            if  (chil is Tekshe chile) {
                child.Add(chile);
                i += GetTotalTekshe(chile);
            }
        }

        float b = MathF.Log2(i);
        for (var index = 0; index < child.Count; index++)
        {
            var c = child[index];
            c.GlobalPosition = tekshe.GlobalPosition 
                               + Vector2.Left  * ratio * i/2
                               + Vector2.Right * ratio * i * index 
                               + Vector2.Down * 90;
        }

        return i;
    }
}
