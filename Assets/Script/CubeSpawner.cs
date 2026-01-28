using UnityEngine;
using Zenject;

public class CubeSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private Cube cubePrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int poolSize = 10;
    [SerializeField] private Transform poolContainer;

    [Header("Borders")]
    [SerializeField] private Transform leftBorder;
    [SerializeField] private Transform rightBorder;

    private ObjectPool<Cube> pool;

    // Залежності, які прийдуть через Zenject
    private IInputHandler inputHandler;
    private ScoreBank scoreBank;

    // Ін'єкція залежностей
    [Inject]
    public void Construct(IInputHandler inputHandler, ScoreBank scoreBank)
    {
        this.inputHandler = inputHandler;
        this.scoreBank = scoreBank;
    }

    private void Awake()
    {
        pool = new ObjectPool<Cube>(cubePrefab, poolSize, poolContainer);
    }

    public Cube Spawn()
    {
        Cube cubeToSpawn = pool.GetElement();

        cubeToSpawn.transform.position = spawnPoint.position;
        cubeToSpawn.ResetCube();
        cubeToSpawn.Init(leftBorder.position.x, rightBorder.position.x);
        cubeToSpawn.Construct(this, scoreBank);

        var presenter = cubeToSpawn.GetComponent<CubeInputPresenter>();
        if (presenter == null) presenter = cubeToSpawn.gameObject.AddComponent<CubeInputPresenter>();
        presenter.enabled = true;
        presenter.Construct(cubeToSpawn, inputHandler);

        float random = Random.value;
        cubeToSpawn.Value = (random > 0.75f) ? 4 : 2;
        cubeToSpawn.UpdateVisuals();

        return cubeToSpawn;
    }

    public Cube SpawnSpecific(Vector3 position, int value)
    {
        Cube cubeToSpawn = pool.GetElement();

        cubeToSpawn.transform.position = position;
        cubeToSpawn.ResetCube();
        cubeToSpawn.Init(leftBorder.position.x, rightBorder.position.x);
        cubeToSpawn.Construct(this, scoreBank);
        cubeToSpawn.Value = value;
        cubeToSpawn.UpdateVisuals();

        return cubeToSpawn;
    }

    public void ReturnToPool(Cube cube)
    {
        pool.ReturnElement(cube);
    }
}