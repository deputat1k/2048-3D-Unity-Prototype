using System.Collections;
using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{

    [Header("UI & Level")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private LevelLoader levelLoader;
    [SerializeField] private GameSettings settings;

  
    private IInputHandler inputHandler;
    private CubeSpawner cubeSpawner;
    private DeadZone deadZone;
    private Cube currentCubeScript;
    private bool isGameOver = false;

    [Inject]
    public void Construct(IInputHandler input, CubeSpawner spawner, DeadZone deadZone)
    {
        this.inputHandler = input;
        this.cubeSpawner = spawner;
        this.deadZone = deadZone;
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

    private void SpawnNewCube()
    {
        if (isGameOver) return;
        currentCubeScript = cubeSpawner.Spawn();
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