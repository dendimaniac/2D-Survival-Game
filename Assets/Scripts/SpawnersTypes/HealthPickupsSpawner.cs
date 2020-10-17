using PickupsTypes;
using UnityEngine;
using Zenject;

namespace SpawnersTypes
{
    public class HealthPickupsSpawner : PickupsSpawner
    {
        private Pickups.Factory _healthPickupsFactory;

        [Inject]
        private void Construct(Pickups.Factory healthPickupsFactory)
        {
            _healthPickupsFactory = healthPickupsFactory;
        }
    
        protected override GameObject RandomObject()
        {
            return _healthPickupsFactory.Create().gameObject;
        }
    }
}