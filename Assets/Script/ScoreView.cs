using TMPro;
using UnityEngine;

public class ScoreView : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;

    private void Start()
    {
        if (ScoreBank.Instance != null)
        {
            ScoreBank.Instance.OnScoreChanged += UpdateScoreText;
            UpdateScoreText(ScoreBank.Instance.GetScore());
        }
    }

    private void OnDestroy()
    {
        //відписуємось, коли об'єкт знищується
        if (ScoreBank.Instance != null)
        {
            ScoreBank.Instance.OnScoreChanged -= UpdateScoreText;
        }
    }

    // Цей метод викличеться коли Банк крикне "OnScoreChanged"
    private void UpdateScoreText(int newScore)
    {
        scoreText.text = "Score: " + newScore;
    }
}