using UnityEngine;

namespace Cube2048.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject gameOverPanel;

        private void Start()
        {
            if (gameOverPanel != null)
                gameOverPanel.SetActive(false);
        }

        public void ShowGameOver()
        {
            if (gameOverPanel != null)
                gameOverPanel.SetActive(true);
        }
    }
}