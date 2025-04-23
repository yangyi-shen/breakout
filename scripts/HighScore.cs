using Godot;
using System;

public partial class HighScore : Resource
{
    [Export]
    public int Score { get; set; }

    public HighScore() : this(0) {}

    public HighScore(int Score)
    {
        this.Score = Score;
    }
}
