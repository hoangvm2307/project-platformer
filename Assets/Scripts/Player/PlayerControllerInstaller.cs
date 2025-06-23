using UnityEngine;
using Zenject;

public class PlayerControllerInstaller : Installer<PlayerControllerInstaller>
{
    public override void InstallBindings()
    {
        Container.Bind<Rigidbody2D>().FromComponentOnRoot().AsSingle();
        Container.Bind<Transform>().FromComponentOnRoot().AsSingle();
        Container.Bind<PlayerCollisionHandler>().FromComponentOnRoot().AsSingle();
        Container.BindInterfacesAndSelfTo<PlayerController>().AsSingle();
    }
}