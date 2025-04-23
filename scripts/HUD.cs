using Godot;
using System.Text.RegularExpressions;

public partial class HUD : CanvasLayer
{
	[Export] public HighScore HighScore;

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		SyncHighScore();
	}

	private void OnGameScore()
	{
		// increase score by 100 everytime brick broken
		Label scoreLabel = GetNode<Label>("label_score");
		int currentScore = int.Parse(scoreLabel.Text.Split(": ")[1]);
		currentScore += 100;
		scoreLabel.Text = $"SCORE: {currentScore}";

		// if appropriate, update high score
		if (currentScore > HighScore.Score)
		{
			HighScore.Score = currentScore;
			SyncHighScore();
		}
	}

	private void OnGameLoseLife()
	{
		// decrease life by 1 every time ball passes screen bottom
		Label livesLabel = GetNode<Label>("label_lives");
		int currentLives = Regex.Matches(livesLabel.Text, "x").Count;
		currentLives--;

		if (currentLives <= 0) // handle game over
		{
			livesLabel.Text = "GAME OVER";
		}
		else // handle life reduced
		{
			livesLabel.Text = $"{livesLabel.Text.Substring(0, livesLabel.Text.Length - 2)}";
		}
	}

	private void SyncHighScore()
	{
		// load latest high score and display
		Label highscoreLabel = GetNode<Label>("label_highscore");
		highscoreLabel.Text = HighScore.Score.ToString();

		// save current high score to save file
		ResourceSaver.Save(HighScore, "res://resources/HighScore.tres");
	}
}
