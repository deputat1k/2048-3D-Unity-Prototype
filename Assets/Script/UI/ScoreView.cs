using TMPro;
using UnityEngine;
using Zenject;
using Cube2048.Core;

namespace Cube2048.UI
{
    public class ScoreView : MonoBehaviour
    {
        [SerializeField] private TMP_Text scoreText;

        private ScoreBank scoreBank;


        [Inject]
        public void Construct(ScoreBank scoreBank)
        {
            this.scoreBank = scoreBank;
        }


        private void Start()
        {
            if (scoreBank != null)
            {
                scoreBank.OnScoreChanged += UpdateScoreText;
                UpdateScoreText(scoreBank.GetScore());
            }
        }

        private void OnDestroy()
        {

            if (scoreBank != null)
            {
                scoreBank.OnScoreChanged -= UpdateScoreText;
            }
        }

        private void UpdateScoreText(int newScore)
        {
            scoreText.text = "Score: " + newScore;
        }
    }
}