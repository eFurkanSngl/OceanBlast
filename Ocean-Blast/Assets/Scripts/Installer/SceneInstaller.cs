using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<TilePool>().FromComponentInHierarchy().AsSingle();
        Container.Bind<LauncherManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<TrailPool>().FromComponentInHierarchy().AsSingle();
        InstallSignalBus();
        Container.DeclareSignal<ClickSoundSignals>();
        Container.DeclareSignal<AnimSignalBus>();
    }


    private void InstallSignalBus()
    {
        SignalBusInstaller.Install(Container);
    }
}
