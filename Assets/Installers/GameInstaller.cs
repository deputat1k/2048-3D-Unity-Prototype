using UnityEngine;
using Zenject;
using Cube2048.Core.Interfaces;
using Cube2048.Features.AutoMerge;
using Cube2048.Features.AutoMerge.Strategies;
using Cube2048.Gameplay;
using Cube2048.Input;
using Cube2048.Core;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private CubeSpawner cubeSpawner;
    [SerializeField] private ScoreBank scoreBank;
    [SerializeField] private MergeFXController mergeFXController;
    [SerializeField] private AutoMergeController autoMergeController;
    [SerializeField] private MergeProcessor mergeProcessor;
    [SerializeField] private InputHandler inputHandler;

    public override void InstallBindings()
    {
        // 1. Spawner
        Container.Bind<ICubeSpawner>().FromInstance(cubeSpawner).AsSingle();

        // 2. Score
        Container.Bind<IScoreService>().FromInstance(scoreBank).AsSingle();

        // 3. FX
        Container.Bind<IMergeFX>().FromInstance(mergeFXController).AsSingle();

        // 4. AutoMerge Service
        Container.Bind<IAutoMergeService>().To<AutoMergeController>().FromInstance(autoMergeController).AsSingle();

        // 5. Processor (Ми його тепер інжектимо в контролер)
        Container.Bind<MergeProcessor>().FromInstance(mergeProcessor).AsSingle();

        // 6. Input
        Container.Bind<IInputHandler>().FromInstance(inputHandler).AsSingle();

        // 7.  СТРАТЕГІЯ (Важливо для AutoMergeController)
        Container.Bind<IMergeStrategy>().To<NearestMergeStrategy>().AsSingle();
    }
}