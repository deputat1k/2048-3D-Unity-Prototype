using Cube2048.Core;
using Cube2048.Core.Interfaces; // Підключаємо інтерфейси
using Cube2048.Features.AutoMerge;
using Cube2048.Gameplay;
using Cube2048.Input;
using UnityEngine;
using Zenject;

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
        // 1. Біндимо Інтерфейс до Компонента
        Container.Bind<ICubeSpawner>().FromInstance(cubeSpawner).AsSingle();

        // 2. Рахунок
        Container.Bind<IScoreService>().FromInstance(scoreBank).AsSingle();

        // 3. FX
        Container.Bind<IMergeFX>().FromInstance(mergeFXController).AsSingle();

        // 4. AutoMerge (біндимо і як інтерфейс, і як клас, якщо треба прямий доступ)
        Container.Bind<IAutoMergeService>().To<AutoMergeController>().FromInstance(autoMergeController).AsSingle();

        // 5. Processor залишаємо поки як є (це пофіксимо в наступних етапах)
        Container.Bind<MergeProcessor>().FromInstance(mergeProcessor).AsSingle();

        Container.Bind<IInputHandler>().FromInstance(inputHandler).AsSingle();

    }
}