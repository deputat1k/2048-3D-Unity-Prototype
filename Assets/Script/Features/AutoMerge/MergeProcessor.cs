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
        [SerializeField] private float cameraOffsetDistance = 2.0f;
        [SerializeField] private Camera mainCamera;

        // 🔥 Залишили ТІЛЬКИ те, що потрібно для візуалу та виклику злиття
        private IMergeFX fxService;
        private IMergeService mergeService;

        [Inject]
        // 🔥 ПРАВИЛЬНИЙ CONSTRUCT (тільки FX та MergeService)
        public void Construct(IMergeFX fxService, IMergeService mergeService)
        {
            this.fxService = fxService;
            this.mergeService = mergeService;
        }

        public async UniTask PerformMergeSequence(Cube cubeA, Cube cubeB)
        {
            if (cubeA == null || cubeB == null) return;

            DisablePhysics(cubeA);
            DisablePhysics(cubeB);

            float sizeA = cubeA.transform.localScale.x;
            float sizeB = cubeB.transform.localScale.x;

            float autoThreshold = ((sizeA / 2f) + (sizeB / 2f)) * 0.9f;

            Vector3 startPosA = cubeA.transform.position;
            Vector3 startPosB = cubeB.transform.position;
            Vector3 centerPos = (startPosA + startPosB) / 2f;
            Vector3 targetPos = centerPos + Vector3.up * settings.LiftHeight;

            await AnimateMoveToTouch(cubeA, cubeB, targetPos, autoThreshold);

            PlayMergeEffect(targetPos);

            if (cubeA == null || cubeB == null) return;

            // Тепер mergeService не null, і все спрацює ідеально
            mergeService.ProcessMerge(cubeA, cubeB, targetPos);
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

            while (elapsed < settings.MergeAnimDuration)
            {
                if (cubeA == null || cubeB == null) return;

                elapsed += Time.deltaTime;
                float t = elapsed / settings.MergeAnimDuration;

                cubeA.transform.position = Vector3.Lerp(startA, targetPos, t);
                cubeB.transform.position = Vector3.Lerp(startB, targetPos, t);

                float currentDistance = Vector3.Distance(cubeA.transform.position, cubeB.transform.position);

                if (currentDistance <= stopDistance)
                {
                    return;
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