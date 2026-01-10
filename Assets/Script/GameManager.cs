using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Components")]
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private GameObject gameOverPanel; // Посилання на панель програшу

    [Header("Settings")]
    [SerializeField] private float moveSpeed = 0.02f;
    [SerializeField] private float xLimit = 1.5f;
    [SerializeField] private float pushForce = 20f;

    private GameObject currentCube;
    private Rigidbody currentRb;
    private bool isGameOver = false;

    private void Awake()
    {
        // Налаштування Singleton
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
        // Ховаємо панель програшу на старті
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        SpawnNewCube();
    }

    private void MoveCube(float deltaX)
    {
        if (isGameOver || currentCube == null) return;

        Vector3 pos = currentCube.transform.position;
        pos.x += deltaX * moveSpeed * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, -xLimit, xLimit);
        currentCube.transform.position = pos;
    }

    private void ShootCube()
    {
        if (isGameOver || currentCube == null) return;

        if (currentRb != null)
        {
            currentRb.AddForce(Vector3.forward * pushForce, ForceMode.Impulse);
        }

        currentCube = null;
        Invoke(nameof(SpawnNewCube), 1f);
    }

    private void SpawnNewCube()
    {
        if (isGameOver) return;

        currentCube = Instantiate(cubePrefab, spawnPoint.position, Quaternion.identity);
        currentRb = currentCube.GetComponent<Rigidbody>();

        // Логіка шансу 75% / 25%
        Cube cubeScript = currentCube.GetComponent<Cube>();
        if (cubeScript != null)
        {
            float random = UnityEngine.Random.value;
            cubeScript.Value = (random > 0.75f) ? 4 : 2;
            cubeScript.UpdateVisuals();
        }
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("Game Over!");

        // Показуємо панель
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        // Перезавантажує поточну сцену
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}