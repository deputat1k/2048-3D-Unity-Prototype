using Cube2048.Core;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Cube2048.Core.Interfaces;

namespace Cube2048.Gameplay
{
    public class CubeSpawner : MonoBehaviour, ICubeSpawner
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
        private DiContainer container;

        public List<Cube> ActiveCubes { get; private set; } = new List<Cube>();

        [Inject]
        public void Construct(DiContainer container)
        {
            this.container = container;
        }

        private void Awake()
        {
            pool = new ObjectPool<Cube>(cubePrefab, poolSize, poolContainer, container);
        }

        public Cube Spawn()
        {
            Cube cubeToSpawn = pool.GetElement();
            ActiveCubes.Add(cubeToSpawn);

            cubeToSpawn.transform.position = spawnPoint.position;
            cubeToSpawn.ResetCube();
            cubeToSpawn.Init(leftBorder.position.x, rightBorder.position.x);

            var presenter = cubeToSpawn.GetComponent<CubeInputPresenter>();
            if (presenter != null) presenter.enabled = true;

            // 🔥 ВИПРАВЛЕНО: Використовуємо метод SetValue замість прямого присвоєння
            int startValue = (Random.value > 0.75f) ? 4 : 2;
            cubeToSpawn.SetValue(startValue);

            // cubeToSpawn.UpdateVisuals(); // Це можна прибрати, бо SetValue вже оновлює візуал

            return cubeToSpawn;
        }

        public Cube SpawnSpecific(Vector3 position, int value)
        {
            Cube cubeToSpawn = pool.GetElement();
            ActiveCubes.Add(cubeToSpawn);

            cubeToSpawn.transform.position = position;
            cubeToSpawn.ResetCube();
            cubeToSpawn.Init(leftBorder.position.x, rightBorder.position.x);

            var presenter = cubeToSpawn.GetComponent<CubeInputPresenter>();
            if (presenter != null) presenter.enabled = false;

            // 🔥 ВИПРАВЛЕНО: Використовуємо метод SetValue
            cubeToSpawn.SetValue(value);

            // cubeToSpawn.UpdateVisuals(); // Це теж можна прибрати

            return cubeToSpawn;
        }

        public void ReturnToPool(Cube cube)
        {
            if (ActiveCubes.Contains(cube))
            {
                ActiveCubes.Remove(cube);
            }
            pool.ReturnElement(cube);
        }
    }
}