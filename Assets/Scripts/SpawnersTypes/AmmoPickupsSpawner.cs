using PickupsTypes;
using UnityEngine;
using Zenject;

namespace SpawnersTypes
{
    public class AmmoPickupsSpawner : PickupsSpawner
    {
        private Pickups.Factory _ammoPickupsFactory;

        [Inject]
        private void Construct(Pickups.Factory ammoPickupsFactory)
        {
            _ammoPickupsFactory = ammoPickupsFactory;
        }
        
        protected override GameObject RandomObject()
        {
            return _ammoPickupsFactory.Create().gameObject;
        }
    }
}