using Cube2048.Core;
using Cube2048.Core.Interfaces;
using Cube2048.Data;
using Cube2048.Gameplay;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Cube2048.Features.AutoMerge
{
    public class AutoMergeController : MonoBehaviour, IAutoMergeService
    {
        [Header("Modules")]
        [SerializeField] private MergeVisuals visuals;
        [SerializeField] private MergeProcessor processor;
        [SerializeField] private LightningSettings settings;

        private ICubeSpawner spawner;
        private bool isRunning = true;

        // Стан системи
        private Cube bestCubeA;
        private Cube bestCubeB;
        public bool IsMerging { get; private set; } = false;
        public bool HasPair => bestCubeA != null && bestCubeB != null;

        [Inject]
        public void Construct(ICubeSpawner spawner)
        {
            this.spawner = spawner;
        }

        private void Start()
        {
            if (visuals == null || processor == null || settings == null)
            {
                Debug.LogError("AutoMergeController: Modules missing!");
                return;
            }
            FindBestPairLoop().Forget();
            UpdateVisualsLoop().Forget();
        }

        private void OnDestroy() => isRunning = false;

        private async UniTaskVoid FindBestPairLoop()
        {
            List<CubeData> dataSnapshot = new List<CubeData>();

            while (isRunning)
            {
                if (IsMerging)
                {
                    visuals.HideLightning();
                    await UniTask.Delay(100);
                    continue;
                }

                dataSnapshot.Clear();
                foreach (var cube in spawner.ActiveCubes)
                {
                    if (cube == null || !cube.gameObject.activeInHierarchy) continue;
                    var presenter = cube.GetComponent<CubeInputPresenter>();
                    if (presenter != null && presenter.enabled) continue;
                    dataSnapshot.Add(new CubeData(cube.GetInstanceID(), cube.transform.position, cube.Value));
                }

                await UniTask.RunOnThreadPool(() =>
                {
                    MathUtils.FindBestPair(dataSnapshot, out int idA, out int idB);
                    return (idA, idB);
                }).ContinueWith(result =>
                {
                    bestCubeA = spawner.ActiveCubes.Find(c => c.GetInstanceID() == result.idA);
                    bestCubeB = spawner.ActiveCubes.Find(c => c.GetInstanceID() == result.idB);
                });

                await UniTask.Delay((int)(settings.CheckInterval * 1000));
            }
        }

        private async UniTaskVoid UpdateVisualsLoop()
        {
            while (isRunning)
            {
                if (!IsMerging && HasPair)
                {
                    visuals.ShowLightning(bestCubeA.transform.position, bestCubeB.transform.position);
                }
                else
                {
                    visuals.HideLightning();
                }
                await UniTask.Yield();
            }
        }

        public async UniTask TriggerMerge()
        {
            if (!HasPair || IsMerging) return;

            IsMerging = true;
            visuals.HideLightning();
            await processor.PerformMergeSequence(bestCubeA, bestCubeB);

            await UniTask.Delay(500);
            bestCubeA = null;
            bestCubeB = null;
            IsMerging = false;
        }
    }
}