using UnityEngine;
using System;

public class ScoreBank : MonoBehaviour
{


    public event Action<int> OnScoreChanged; 

    private int score;

    public int GetScore() => score;

    public void AddScore(int amount)
    {
        score += amount;

        OnScoreChanged?.Invoke(score);
    }

    public void ResetScore()
    {
        score = 0;
        OnScoreChanged?.Invoke(score);
    }
}