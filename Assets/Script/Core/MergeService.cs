using UnityEngine;
using Zenject;
using Cube2048.Core.Interfaces;

namespace Cube2048.Gameplay
{
    public class MergeService : IMergeService
    {
        private readonly ICubeSpawner spawner;
        private readonly IScoreService scoreService;

        [Inject]
        public MergeService(ICubeSpawner spawner, IScoreService scoreService)
        {
            this.spawner = spawner;
            this.scoreService = scoreService;
        }

        public void ProcessMerge(Cube cubeA, Cube cubeB, Vector3 spawnPosition)
        {
            if (cubeA == null || cubeB == null) return;
            if (cubeA.IsMerged || cubeB.IsMerged) return;

            // Позначаємо, що вони вже в процесі злиття, щоб уникнути багів
            cubeA.MarkAsMerged();
            cubeB.MarkAsMerged();

            int newValue = cubeA.Value * 2;

            // Ховаємо старі куби
            spawner.ReturnToPool(cubeA);
            spawner.ReturnToPool(cubeB);

            // Спавнимо новий
            Cube newCube = spawner.SpawnSpecific(spawnPosition, newValue);
            if (newCube != null)
            {
                newCube.Bounce();
            }

            // Нараховуємо очки ТІЛЬКИ ТУТ
            scoreService?.AddScore(newValue);
        }
    }
}