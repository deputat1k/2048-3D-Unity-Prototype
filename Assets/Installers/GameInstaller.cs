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
        
        Container.Bind<ICubeSpawner>()
            .FromInstance(cubeSpawner)
            .AsSingle();

        
        Container.Bind<IScoreService>()
            .FromInstance(scoreBank)
            .AsSingle();

        
        Container.Bind<IMergeFX>()
            .FromInstance(mergeFXController)
            .AsSingle();

     
        Container.Bind<IAutoMergeService>()
            .To<AutoMergeController>()
            .FromInstance(autoMergeController)
            .AsSingle();

       
        Container.Bind<MergeProcessor>()
            .FromInstance(mergeProcessor)
            .AsSingle();

      
        Container.Bind<IInputHandler>()
            .FromInstance(inputHandler)
            .AsSingle();

       
        Container.Bind<IMergeStrategy>()
            .To<NearestMergeStrategy>()
            .AsSingle();

        Container.Bind<IMergeService>()
            .To<MergeService>()
            .AsSingle();
    }
}