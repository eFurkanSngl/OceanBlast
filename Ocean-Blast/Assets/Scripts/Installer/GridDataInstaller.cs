using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName ="GridDataInsta",menuName ="Installer/GridDataInstaller")]
public class GridDataInstaller : ScriptableObjectInstaller<GridDataInstaller>
{
    [SerializeField] private GridData _gridData;

    public override void InstallBindings()
    {
        Container.Bind<GridData>().FromInstance(_gridData).AsSingle();
    }

}
