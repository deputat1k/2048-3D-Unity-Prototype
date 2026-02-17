using System;
using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;
using Cube2048.Data;
using Cube2048.Gameplay;
using Cube2048.Core.Interfaces; // Підключаємо інтерфейси

namespace Cube2048.Features.AutoMerge
{
    public class MergeProcessor : MonoBehaviour
    {
        [SerializeField] private LightningSettings settings;
        [SerializeField] private float mergeContactThreshold = 0.9f;
        [SerializeField] private float cameraOffsetDistance = 2.0f;

        // 🔥 ВИКОРИСТОВУЄМО ІНТЕРФЕЙСИ
        private ICubeSpawner spawner;
        private IScoreService scoreService; // Було scoreBank
        private IMergeFX fxService;         // Було fxController
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

            Vector3 startPosA = cubeA.transform.position;
            Vector3 startPosB = cubeB.transform.position;
            Vector3 centerPos = (startPosA + startPosB) / 2f;
            Vector3 targetPos = centerPos + Vector3.up * settings.LiftHeight;

            await AnimateMoveToTarget(cubeA, cubeB, targetPos, () =>
            {
                PlayMergeEffect(targetPos);
            });

            if (cubeA == null || cubeB == null) return;

            cubeA.gameObject.SetActive(false);
            cubeB.gameObject.SetActive(false);

            spawner.ReturnToPool(cubeA);
            spawner.ReturnToPool(cubeB);

            // 🔥 ВИКЛИКАЄМО ЧЕРЕЗ ІНТЕРФЕЙС
            scoreService?.AddScore(1);

            int newValue = cubeA.Value * 2;
            Cube newCube = spawner.SpawnSpecific(targetPos, newValue);

            if (newCube != null)
            {
                newCube.Bounce();
            }
        }

        private void PlayMergeEffect(Vector3 centerPosition)
        {
            // 🔥 ВИКЛИКАЄМО ЧЕРЕЗ ІНТЕРФЕЙС (fxService замість fxController)
            if (fxService == null) return;

            Vector3 spawnPos = centerPosition;

            if (mainCamera != null)
            {
                Vector3 directionToCamera = (mainCamera.transform.position - centerPosition).normalized;
                spawnPos = centerPosition + (directionToCamera * cameraOffsetDistance);
            }

            fxService.PlayExplosion(spawnPos);
        }

        private async UniTask AnimateMoveToTarget(Cube cubeA, Cube cubeB, Vector3 targetPos, Action onContact)
        {
            float elapsed = 0f;
            Vector3 startA = cubeA.transform.position;
            Vector3 startB = cubeB.transform.position;
            bool hasTriggeredContact = false;

            while (elapsed < settings.MergeAnimDuration)
            {
                if (cubeA == null || cubeB == null) return;

                elapsed += Time.deltaTime;
                float t = elapsed / settings.MergeAnimDuration;

                cubeA.transform.position = Vector3.Lerp(startA, targetPos, t);
                cubeB.transform.position = Vector3.Lerp(startB, targetPos, t);

                float currentDistance = Vector3.Distance(cubeA.transform.position, cubeB.transform.position);

                if (!hasTriggeredContact && currentDistance <= mergeContactThreshold)
                {
                    onContact?.Invoke();
                    hasTriggeredContact = true;
                }

                await UniTask.Yield();
            }

            if (!hasTriggeredContact) onContact?.Invoke();
        }

        private void DisablePhysics(Cube cube)
        {
            var rb = cube.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;
        }
    }
}