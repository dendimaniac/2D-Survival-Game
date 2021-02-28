using CustomFactories;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class EnemySpawnerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindFactory<Object, Vector3, Quaternion, Transform, Transform, TransformFactory>()
                .FromFactory<CustomTransformFactory>();
        }
    }
}