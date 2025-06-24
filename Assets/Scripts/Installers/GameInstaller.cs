using GameManagement;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [Header("Prefabs")]
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _arrowPrefab;

    [Header("Spawning")]
    [SerializeField] private Transform _playerSpawnPoint;

    [Header("Settings")]
    [SerializeField] private PlayerSettings _playerSettings;

    [Header("UI")]
    [SerializeField] private UIManager _uiManager;

    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);

        Container.DeclareSignal<PlayerDiedSignal>();
        Container.DeclareSignal<EnemyDiedSignal>();
        Container.DeclareSignal<RestartGameSignal>();
        Container.DeclareSignal<EnemySpawnedSignal>();
        Container.DeclareSignal<PlayerLaunchSignal>();
        Container.DeclareSignal<GameStateChangedSignal>();
        
        Container.Bind<GameObject>().WithId("ArrowPrefab").FromInstance(_arrowPrefab);
        Container.Bind<PlayerSettings>().FromInstance(_playerSettings).AsSingle();
        Container.Bind<Camera>().FromComponentInHierarchy().AsSingle();

        Container.Bind<UIManager>().FromInstance(_uiManager).AsSingle();
        Container.BindInterfacesAndSelfTo<GameManager>().AsSingle().NonLazy();

        Container.Bind<PlayerController>()
            .FromSubContainerResolve()
            .ByNewPrefabInstaller<PlayerControllerInstaller>(_playerPrefab)
            .WithGameObjectName("Player")
            .UnderTransform(_playerSpawnPoint)
            .AsSingle()
            .NonLazy();

        Container.BindInterfacesAndSelfTo<InputManager>().AsSingle().NonLazy();
    }
}