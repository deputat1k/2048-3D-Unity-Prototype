using System;
using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;
using Cube2048.Data;
using Cube2048.Gameplay;
using Cube2048.Core.Interfaces;

namespace Cube2048.Features.AutoMerge
{
    public class MergeProcessor : MonoBehaviour
    {
        [SerializeField] private LightningSettings settings;

        // Ми прибрали ручне налаштування дистанції. Все автоматично.
        [SerializeField] private float cameraOffsetDistance = 2.0f;

        private ICubeSpawner spawner;
        private IScoreService scoreService;
        private IMergeFX fxService;
        private Camera mainCamera;

        [Inject]
        public void Construct(ICubeSpawner spawner, IScoreService scoreService, IMergeFX fxService)
        {
            this.spawner = spawner;
            this.scoreService = scoreService;
            this.fxService = fxService;
            this.mainCamera = Camera.main;
        }

        public async UniTask PerformMergeSequence(Cube cubeA, Cube cubeB)
        {
            if (cubeA == null || cubeB == null) return;

            DisablePhysics(cubeA);
            DisablePhysics(cubeB);

            // 🔥 АВТОМАТИЧНИЙ РОЗРАХУНОК:
            // Дистанція дотику = (половина ширини A) + (половина ширини B).
            // Якщо куби розміром 1, то 0.5 + 0.5 = 1.0 (вони торкаються центрами).
            float sizeA = cubeA.transform.localScale.x;
            float sizeB = cubeB.transform.localScale.x;

            // Множимо на 0.9, щоб вони трішки перекрили один одного (ефект удару)
            float autoThreshold = ((sizeA / 2f) + (sizeB / 2f)) * 0.9f;

            Vector3 startPosA = cubeA.transform.position;
            Vector3 startPosB = cubeB.transform.position;
            Vector3 centerPos = (startPosA + startPosB) / 2f;
            Vector3 targetPos = centerPos + Vector3.up * settings.LiftHeight;

            // Передаємо вирахуваний поріг у метод руху
            await AnimateMoveToTouch(cubeA, cubeB, targetPos, autoThreshold);

            // Ефект і логіка заміни
            PlayMergeEffect(targetPos);

            if (cubeA == null || cubeB == null) return;

            cubeA.gameObject.SetActive(false);
            cubeB.gameObject.SetActive(false);

            spawner.ReturnToPool(cubeA);
            spawner.ReturnToPool(cubeB);

            scoreService?.AddScore(cubeA.Value * 2);

            int newValue = cubeA.Value * 2;
            Cube newCube = spawner.SpawnSpecific(targetPos, newValue);

            if (newCube != null)
            {
                newCube.Bounce();
            }
        }

        private void PlayMergeEffect(Vector3 centerPosition)
        {
            if (fxService == null) return;

            Vector3 spawnPos = centerPosition;
            if (mainCamera != null)
            {
                Vector3 directionToCamera = (mainCamera.transform.position - centerPosition).normalized;
                spawnPos = centerPosition + (directionToCamera * cameraOffsetDistance);
            }

            fxService.PlayExplosion(spawnPos);
        }

        private async UniTask AnimateMoveToTouch(Cube cubeA, Cube cubeB, Vector3 targetPos, float stopDistance)
        {
            float elapsed = 0f;
            Vector3 startA = cubeA.transform.position;
            Vector3 startB = cubeB.transform.position;

            // Страховка: якщо анімація зависне, вона примусово закінчиться через заданий час
            while (elapsed < settings.MergeAnimDuration)
            {
                if (cubeA == null || cubeB == null) return;

                elapsed += Time.deltaTime;
                float t = elapsed / settings.MergeAnimDuration;

                cubeA.transform.position = Vector3.Lerp(startA, targetPos, t);
                cubeB.transform.position = Vector3.Lerp(startB, targetPos, t);

                // 🔥 ДИНАМІЧНА ПЕРЕВІРКА:
                float currentDistance = Vector3.Distance(cubeA.transform.position, cubeB.transform.position);

                // Використовуємо автоматично вирахувану дистанцію
                if (currentDistance <= stopDistance)
                {
                    return; // Миттєвий вихід -> миттєвий мердж
                }

                await UniTask.Yield();
            }
        }

        private void DisablePhysics(Cube cube)
        {
            var rb = cube.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;
        }
    }
}