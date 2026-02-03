using UnityEngine;
using Zenject;
using Cube2048.Core;


namespace Cube2048.Gameplay
{
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
        private DiContainer container;

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

            cubeToSpawn.transform.position = spawnPoint.position;
            cubeToSpawn.ResetCube();
            cubeToSpawn.Init(leftBorder.position.x, rightBorder.position.x);

            
            var presenter = cubeToSpawn.GetComponent<CubeInputPresenter>();
            if (presenter != null) presenter.enabled = true;

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

            var presenter = cubeToSpawn.GetComponent<CubeInputPresenter>();
            if (presenter != null) presenter.enabled = false;

            cubeToSpawn.Value = value;
            cubeToSpawn.UpdateVisuals();

            return cubeToSpawn;
        }

        public void ReturnToPool(Cube cube)
        {
            pool.ReturnElement(cube);
        }
    }
}