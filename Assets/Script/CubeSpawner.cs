using System.Collections.Generic;
using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    public static CubeSpawner Instance;

    [Header("Spawn Settings")]
    [SerializeField] private Cube cubePrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int poolSize = 10; // Скільки кубиків заготувати на старті


    private Queue<Cube> cubePool = new Queue<Cube>();

    private void Awake()
    {
        if (Instance == null) Instance = this;

  
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            Cube newCube = Instantiate(cubePrefab, spawnPoint.position, Quaternion.identity);
            newCube.gameObject.SetActive(false); 
            cubePool.Enqueue(newCube);
        }
    }


    public Cube Spawn()
    {
        Cube cubeToSpawn;

        //  Перевіряємо, чи є вільні кубики
        if (cubePool.Count > 0)
        {
            cubeToSpawn = cubePool.Dequeue();
        }
        else
        {
            // Якщо не вистачило - створюємо новий 
            cubeToSpawn = Instantiate(cubePrefab);
        }

        //  Ставимо його на позицію старту
        cubeToSpawn.transform.position = spawnPoint.position;

        cubeToSpawn.ResetCube();

        //  рандом чисел
        float random = Random.value;
        cubeToSpawn.Value = (random > 0.75f) ? 4 : 2;
        cubeToSpawn.UpdateVisuals();

        return cubeToSpawn;
    }


    public void ReturnToPool(Cube cube)
    {
        cube.Deactivate();
        cubePool.Enqueue(cube); // Кладемо назад в кінець черги
    }
    public Cube SpawnSpecific(Vector3 position, int value)
    {
        Cube cubeToSpawn;

        if (cubePool.Count > 0)
        {
            cubeToSpawn = cubePool.Dequeue();
        }
        else
        {
            cubeToSpawn = Instantiate(cubePrefab);
        }

        cubeToSpawn.transform.position = position;
        cubeToSpawn.ResetCube();

        // Ставимо конкретне значення
        cubeToSpawn.Value = value;
        cubeToSpawn.UpdateVisuals();

        return cubeToSpawn;
    }
}