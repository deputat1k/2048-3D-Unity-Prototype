using UnityEngine;
using Zenject;
using Cube2048.Gameplay; 
using Cube2048.Input;    
using Cube2048.Core;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private CubeSpawner cubeSpawner;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private DeadZone deadZone;
    [SerializeField] private ScoreBank scoreBank;

    public override void InstallBindings()
    {
        Container.Bind<IInputHandler>()
            .FromInstance(inputHandler)
            .AsSingle();

        Container.Bind<CubeSpawner>()
            .FromInstance(cubeSpawner)
            .AsSingle();

        Container.Bind<GameManager>()
            .FromInstance(gameManager)
            .AsSingle();

        Container.Bind<DeadZone>()
            .FromInstance(deadZone)
            .AsSingle();

        Container.Bind<ScoreBank>()
            .FromInstance(scoreBank)
            .AsSingle();
    }
}