using PickupsTypes;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class PickupSpawnerInstaller : MonoInstaller
    {
        [SerializeField] private Pickups _pickups;

        public override void InstallBindings()
        {
            Container.BindFactory<Pickups, Pickups.Factory>()
                .FromMonoPoolableMemoryPool(binder =>
                    binder.WithInitialSize(5).To<Pickups>()
                        .FromComponentInNewPrefab(_pickups));
        }
    }
}