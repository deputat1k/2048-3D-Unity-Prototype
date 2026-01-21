using System; 
using UnityEngine;

public class ScoreBank : MonoBehaviour
{
    public static ScoreBank Instance;

    //Підписники отримають нове число
    public event Action<int> OnScoreChanged;

    private int score;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void AddScore(int amount)
    {
        score += amount;

        // Сповіщаємо всіх, хто підписаний що рахунок змінився
        OnScoreChanged?.Invoke(score);
    }

    // Метод щоб дізнатися поточний рахунок
    public int GetScore() => score;
}