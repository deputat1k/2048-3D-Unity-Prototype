using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using Cube2048.Core.Interfaces;
using Cube2048.Data;
using Cube2048.Gameplay;

namespace Cube2048.Features.AutoMerge
{
    public class AutoMergeController : MonoBehaviour, IAutoMergeService
    {
        [Header("Visuals")]
        [SerializeField] private MergeVisuals visuals;

        [Header("Settings")]
        [SerializeField] private LightningSettings settings;

        private ICubeSpawner spawner;
        private MergeProcessor processor;
        private IMergeStrategy mergeStrategy;

        private bool isRunning = true;
        private bool isPaused = false;
        private bool _isMerging;

        private Cube bestCubeA;
        private Cube bestCubeB;

        public event Action<bool> OnStatusChanged;
        public bool HasPair => !isPaused && bestCubeA != null && bestCubeB != null && !IsMerging;

        public bool IsMerging
        {
            get => _isMerging;
            private set
            {
                if (_isMerging != value)
                {
                    _isMerging = value;
                    NotifyStatusChanged();
                }
            }
        }

        [Inject]
        public void Construct(ICubeSpawner spawner, IMergeStrategy strategy, MergeProcessor processor)
        {
            this.spawner = spawner;
            this.mergeStrategy = strategy;
            this.processor = processor;
        }

        private void Start()
        {
            FindBestPairLoop().Forget();
            UpdateVisualsLoop().Forget();
        }

        private void OnDestroy() => isRunning = false;

        public void SetPaused(bool paused)
        {
            isPaused = paused;

            if (isPaused)
            {
                if (visuals != null) visuals.HideLightning();
            }

            NotifyStatusChanged();
        }

        private void NotifyStatusChanged()
        {
            bool canInteract = HasPair && !IsMerging;
            OnStatusChanged?.Invoke(canInteract);
        }

        private async UniTask FindBestPairLoop()
        {
            while (this != null && isRunning)
            {
             
                if (!IsMerging && !isPaused)
                {
                   
                    var activeCubes = spawner.ActiveCubes;
                    var snapshot = new List<CubeData>();
                    var currentCubesRef = new List<Cube>();

                    foreach (var cube in activeCubes)
                    {
                        if (cube == null || !cube.gameObject.activeInHierarchy) continue;
                        if (!cube.IsLaunched) continue;

                        snapshot.Add(new CubeData(cube.transform.position, cube.Value, cube.GetInstanceID()));
                        currentCubesRef.Add(cube);
                    }

                    if (snapshot.Count < 2)
                    {
                        UpdatePair(null, null);
                        await UniTask.Delay(TimeSpan.FromSeconds(settings.CheckInterval));
                        continue;
                    }

                    var result = await UniTask.RunOnThreadPool(() =>
                    {
                        return mergeStrategy.FindBestPair(snapshot);
                    });

                    if (result.indexA != -1 && result.indexB != -1 &&
                        result.indexA < currentCubesRef.Count && result.indexB < currentCubesRef.Count)
                    {
                        UpdatePair(currentCubesRef[result.indexA], currentCubesRef[result.indexB]);
                    }
                    else
                    {
                        UpdatePair(null, null);
                    }
                }

                await UniTask.Delay(TimeSpan.FromSeconds(settings.CheckInterval));
            }
        }

        private void UpdatePair(Cube a, Cube b)
        {
            bool wasPair = HasPair;
            bestCubeA = a;
            bestCubeB = b;

        
            if (wasPair != HasPair)
            {
                NotifyStatusChanged();
            }
        }

        private async UniTask UpdateVisualsLoop()
        {
            while (this != null && isRunning)
            {

                bool isPairValid = HasPair &&
                                   bestCubeA != null && bestCubeA.gameObject.activeInHierarchy &&
                                   bestCubeB != null && bestCubeB.gameObject.activeInHierarchy;

                if (isPairValid && !IsMerging && !isPaused && visuals != null)
                {
                    visuals.ShowLightning(bestCubeA.transform.position, bestCubeB.transform.position);
                }
                else if (visuals != null)
                {
                    visuals.HideLightning();

                    if (HasPair && !isPairValid)
                    {
                        bestCubeA = null;
                        bestCubeB = null;
                        NotifyStatusChanged(); 
                    }
                }
                await UniTask.Yield();
            }
        }

        public async UniTask TriggerMerge()
        {

            if (!HasPair || IsMerging || isPaused) return;

            IsMerging = true;
            if (visuals != null) visuals.HideLightning();

            await processor.PerformMergeSequence(bestCubeA, bestCubeB);

            UpdatePair(null, null);

            await UniTask.Delay(200);
            IsMerging = false;
        }
    }
}