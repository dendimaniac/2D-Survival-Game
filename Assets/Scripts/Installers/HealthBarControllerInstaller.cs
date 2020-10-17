using Props;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class HealthBarControllerInstaller : MonoInstaller
    {
        [SerializeField] private HealthBar _healthBar;

        public override void InstallBindings()
        {
            Container.BindFactory<Health, HealthBar, HealthBar.Factory>()
                .FromMonoPoolableMemoryPool(binder =>
                    binder.WithInitialSize(10)
                        .FromComponentInNewPrefab(_healthBar)
                        .WithGameObjectName("HealthBar")
                        .UnderTransform(transform));
        }
    }
}