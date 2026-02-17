using System.Collections;
using UnityEngine;
using Zenject;
using Cube2048.Gameplay;
using Cube2048.UI;
using Cube2048.Data;
using Cube2048.Core.Interfaces; // Тепер це спрацює, бо ми змінили namespace в IInputHandler

namespace Cube2048.Core
{
    public class GameManager : MonoBehaviour
    {
        [Header("UI & Level")]
        [SerializeField] private UIManager uiManager;
        [SerializeField] private LevelLoader levelLoader;
        [SerializeField] private GameSettings settings;

        // DeadZone, якщо він прив'язаний в інспекторі
        [SerializeField] private DeadZone deadZone;

        private IInputHandler inputHandler;
        private ICubeSpawner spawner;

        private Cube currentCubeScript;
        private bool isGameOver = false;

        [Inject]
        public void Construct(ICubeSpawner spawner, IInputHandler inputHandler)
        {
            this.inputHandler = inputHandler;
            this.spawner = spawner;
        }

        private void OnEnable()
        {
            if (inputHandler != null) inputHandler.OnRelease += OnPlayerReleased;
            if (deadZone != null) deadZone.OnZoneFilled += GameOver;
        }

        private void OnDisable()
        {
            if (inputHandler != null) inputHandler.OnRelease -= OnPlayerReleased;
            if (deadZone != null) deadZone.OnZoneFilled -= GameOver;
        }

        private void Start()
        {
            SpawnNewCube();
        }

        private void OnPlayerReleased()
        {
            if (isGameOver || currentCubeScript == null) return;
            currentCubeScript = null;
            StartCoroutine(SpawnRoutine(1f));
        }

        private IEnumerator SpawnRoutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            SpawnNewCube();
        }

        public void SpawnNewCube()
        {
            if (spawner != null)
            {
                // Використовуємо метод інтерфейсу
                currentCubeScript = spawner.Spawn();
            }
            else
            {
                Debug.LogError("[GameManager] Spawner is null! Check Zenject bindings.");
            }
        }

        public void GameOver()
        {
            if (isGameOver) return;
            isGameOver = true;
            if (uiManager != null) uiManager.ShowGameOver();
        }

        public void RestartGame()
        {
            if (levelLoader != null) levelLoader.ReloadCurrentLevel();
        }
    }
}