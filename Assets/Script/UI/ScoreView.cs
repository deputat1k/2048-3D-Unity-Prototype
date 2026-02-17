using UnityEngine;
using Zenject;
using TMPro;
using Cube2048.Core.Interfaces;

namespace Cube2048.UI
{
    public class ScoreView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;

        // Змінюємо ім'я на правильне (інтерфейсне)
        private IScoreService scoreService;

        [Inject]
        public void Construct(IScoreService scoreService)
        {
            // Виправляємо помилку присвоєння (CS1717)
            this.scoreService = scoreService;
        }

        private void Start()
        {
            if (scoreService == null) return;

            // 1. Показуємо поточний рахунок одразу при старті
            UpdateVisuals(scoreService.CurrentScore);

            // 2. Підписуємось на подію (тепер вона є в інтерфейсі!)
            // Як тільки рахунок зміниться -> викличеться метод UpdateVisuals
            scoreService.OnScoreChanged += UpdateVisuals;
        }

        private void OnDestroy()
        {
            // Обов'язково відписуємось, щоб не було помилок пам'яті
            if (scoreService != null)
            {
                scoreService.OnScoreChanged -= UpdateVisuals;
            }
        }

        // Цей метод викликається автоматично
        private void UpdateVisuals(int newScore)
        {
            scoreText.text = $"Score: {newScore}";
        }
    }
}