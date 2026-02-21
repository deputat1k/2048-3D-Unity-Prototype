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
        [SerializeField] private GameObject mergeContainerPrefab;

        [Header("Timings & Height")]

        [SerializeField] private float liftHeight = 1.5f;
        [SerializeField] private float liftDuration = 0.6f;
        [SerializeField] private float animationDuration = 0.5f;

        private IMergeFX fxService;
        private IMergeService mergeService;

        [Inject]
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

            Vector3 centerPos = (cubeA.transform.position + cubeB.transform.position) / 2f;
       
            centerPos += Vector3.up * liftHeight;

            GameObject container = Instantiate(mergeContainerPrefab, centerPos, Quaternion.identity);

            Animator anim = container.GetComponent<Animator>();
            if (anim != null) anim.enabled = false;

            Transform slotA = container.transform.Find("SlotA");
            Transform slotB = container.transform.Find("SlotB");


            Transform targetA, targetB;
            if (cubeA.transform.position.x < cubeB.transform.position.x)
            {
                targetA = slotA;
                targetB = slotB;
            }
            else
            {
                targetA = slotB;
                targetB = slotA;
            }


            float elapsed = 0f;
            Vector3 startPosA = cubeA.transform.position;
            Vector3 startPosB = cubeB.transform.position;

            while (elapsed < liftDuration)
            {
                if (cubeA == null || cubeB == null) return;

                elapsed += Time.deltaTime;
                float t = elapsed / liftDuration;

                float smoothT = t * t * (3f - 2f * t);

                cubeA.transform.position = Vector3.Lerp(startPosA, targetA.position, smoothT);
                cubeB.transform.position = Vector3.Lerp(startPosB, targetB.position, smoothT);

                await UniTask.Yield();
            }

            cubeA.transform.SetParent(targetA);
            cubeA.transform.localPosition = Vector3.zero;
            cubeA.transform.localRotation = Quaternion.identity;

            cubeB.transform.SetParent(targetB);
            cubeB.transform.localPosition = Vector3.zero;
            cubeB.transform.localRotation = Quaternion.identity;

            if (anim != null) anim.enabled = true;

            await UniTask.Delay(TimeSpan.FromSeconds(animationDuration));

            PlayMergeEffect(centerPos);

            if (cubeA != null && cubeB != null)
            {
                cubeA.transform.SetParent(null);
                cubeB.transform.SetParent(null);
                mergeService.ProcessMerge(cubeA, cubeB, centerPos);
            }

            if (container != null) Destroy(container);
        }

        private void PlayMergeEffect(Vector3 centerPosition)
        {
            if (fxService != null) fxService.PlayExplosion(centerPosition);
        }

        private void DisablePhysics(Cube cube)
        {
            var rb = cube.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;
        }
    }
}