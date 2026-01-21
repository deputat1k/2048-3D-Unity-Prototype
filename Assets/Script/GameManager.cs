using System.Collections;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
   

    [Header("Components")]
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private LevelLoader levelLoader;
    [SerializeField] private CubeSpawner cubeSpawner;

    [Header("Game Data")]
    [SerializeField] private GameSettings settings;

    private Cube currentCubeScript;
    
    private bool isGameOver = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        if (inputHandler != null)
        {
            inputHandler.OnDrag += MoveCube;
            inputHandler.OnRelease += ShootCube;
        }
    }

    private void OnDisable()
    {
        if (inputHandler != null)
        {
            inputHandler.OnDrag -= MoveCube;
            inputHandler.OnRelease -= ShootCube;
        }
    }

    private void Start()
    {
        SpawnNewCube();
    }

    private void MoveCube(float deltaX)
    {
        // Якщо куба немає або гра закінчилась - виходимо
        if (isGameOver || currentCubeScript == null) return;
        currentCubeScript.Move(deltaX);
    }

    private void ShootCube()
    {
        if (isGameOver || currentCubeScript == null) return;

        currentCubeScript.Shoot();
        currentCubeScript = null;

        StartCoroutine(SpawnRoutine(1f));
    }

    private IEnumerator SpawnRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnNewCube();
    }

    private void SpawnNewCube()
    {
        if (isGameOver) return;

        // Спавнер повертає скрипт, ми його запам'ятовуємо
        currentCubeScript = cubeSpawner.Spawn();
    }

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        //Просимо менеджера показати екран
        if (uiManager != null) uiManager.ShowGameOver();
    }

    public void RestartGame()
    {
        if (levelLoader != null)
        {
            levelLoader.ReloadCurrentLevel();
        }
    }
}