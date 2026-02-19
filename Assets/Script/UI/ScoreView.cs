using UnityEngine;
using Zenject;
using TMPro;
using Cube2048.Core.Interfaces;

namespace Cube2048.UI
{
    public class ScoreView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;

        private IScoreService scoreService;

        [Inject]
        public void Construct(IScoreService scoreService)
        {
            this.scoreService = scoreService;
        }

        private void Start()
        {
            if (scoreService == null) return;

            UpdateVisuals(scoreService.CurrentScore);

            scoreService.OnScoreChanged += UpdateVisuals;
        }

        private void OnDestroy()
        {
            if (scoreService != null)
            {
                scoreService.OnScoreChanged -= UpdateVisuals;
            }
        }

        private void UpdateVisuals(int newScore)
        {
            scoreText.text = $"Score: {newScore}";
        }
    }
}