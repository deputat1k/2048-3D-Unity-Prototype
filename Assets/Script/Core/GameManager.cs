using System.Collections;
using UnityEngine;
using Zenject;
using Cube2048.Gameplay;
using Cube2048.UI;
using Cube2048.Data;
using Cube2048.Core.Interfaces;

namespace Cube2048.Core
{
    public class GameManager : MonoBehaviour
    {
        [Header("UI & Level")]
        [SerializeField] private UIManager uiManager;
        [SerializeField] private LevelLoader levelLoader;
        [SerializeField] private GameSettings settings;
        [SerializeField] private DeadZone deadZone;

        private IInputHandler inputHandler;
        private ICubeSpawner spawner;

        //  ДОДАЄМО ЗАЛЕЖНІСТЬ
        private IAutoMergeService autoMergeService;

        private Cube currentCubeScript;
        private bool isGameOver = false;

        [Inject]
        //  ОТРИМУЄМО СЕРВІС В КОНСТРУКТОРІ
        public void Construct(ICubeSpawner spawner, IInputHandler inputHandler, IAutoMergeService autoMergeService)
        {
            this.inputHandler = inputHandler;
            this.spawner = spawner;
            this.autoMergeService = autoMergeService;
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
            if (!isGameOver) // Перевірка, щоб не спавнити після програшу
            {
                SpawnNewCube();
            }
        }

        public void SpawnNewCube()
        {
            if (spawner != null)
            {
                currentCubeScript = spawner.Spawn();
            }
        }

        public void GameOver()
        {
            if (isGameOver) return;
            isGameOver = true;

            // ВИМИКАЄМО АВТО-МЕРДЖ
            if (autoMergeService != null)
            {
                autoMergeService.SetPaused(true);
            }

            if (uiManager != null) uiManager.ShowGameOver();
        }

        public void RestartGame()
        {
            // При рестарті (перезавантаженні сцени) все скинеться саме,
            // але якщо ти робиш м'який рестарт без перезавантаження сцени,
            // то треба викликати autoMergeService.SetPaused(false);

            if (levelLoader != null) levelLoader.ReloadCurrentLevel();
        }
    }
}