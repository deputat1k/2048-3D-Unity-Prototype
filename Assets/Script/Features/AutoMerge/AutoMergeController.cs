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

        private ICubeSpawner spawner;
        private MergeProcessor processor;
        private IMergeStrategy mergeStrategy;

        private bool isRunning = true;

        // 🔥 НОВА ЗМІННА: Чи на паузі система?
        private bool isPaused = false;

        private Cube bestCubeA;
        private Cube bestCubeB;

        public event Action<bool> OnStatusChanged;

        private bool _isMerging;
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

        // 🔥 ОНОВЛЕНА ЛОГІКА: Якщо пауза - пари немає
        public bool HasPair => !isPaused && bestCubeA != null && bestCubeB != null && !IsMerging;

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

        // 🔥 РЕАЛІЗАЦІЯ НОВОГО МЕТОДУ
        public void SetPaused(bool paused)
        {
            isPaused = paused;

            // Якщо поставили на паузу - ховаємо блискавку і оновлюємо кнопку
            if (isPaused)
            {
                if (visuals != null) visuals.HideLightning();
            }

            NotifyStatusChanged(); // Кнопка перевірить HasPair і вимкнеться, бо isPaused = true
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
                // 🔥 Якщо пауза - не шукаємо нові пари
                if (!IsMerging && !isPaused)
                {
                    // ... (тут твій старий код пошуку, без змін) ...
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
                        await UniTask.Delay(500);
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

                await UniTask.Delay(500);
            }
        }

        private void UpdatePair(Cube a, Cube b)
        {
            bool wasPair = HasPair;
            bestCubeA = a;
            bestCubeB = b;

            // Якщо стан змінився (включаючи вплив isPaused)
            if (wasPair != HasPair)
            {
                NotifyStatusChanged();
            }
        }

        private async UniTaskVoid UpdateVisualsLoop()
        {
            while (this != null && isRunning)
            {
                // 🔥 Додали перевірку !isPaused
                if (HasPair && !IsMerging && !isPaused && visuals != null)
                {
                    visuals.ShowLightning(bestCubeA.transform.position, bestCubeB.transform.position);
                }
                else if (visuals != null)
                {
                    visuals.HideLightning();
                }
                await UniTask.Yield();
            }
        }

        public async UniTask TriggerMerge()
        {
            // 🔥 Захист: якщо пауза - не зливаємо
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