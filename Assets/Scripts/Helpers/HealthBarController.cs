using System.Collections.Generic;
using Props;
using UnityEngine;
using Zenject;

namespace Helpers
{
    public class HealthBarController : MonoBehaviour
    {
        private readonly Dictionary<Health, HealthBar> _healthBars = new Dictionary<Health, HealthBar>();
        private HealthBar.Pool _healthBarPool;

        [Inject]
        public void Construct(HealthBar.Pool healthBarPool)
        {
            _healthBarPool = healthBarPool;
        }

        private void Awake()
        {
            _healthBars.Clear();
            Health.OnHealthAdded += AddHealthBar;
            Health.OnHealthRemoved += RemoveHealthBar;
        }

        private void AddHealthBar(Health health)
        {
            if (_healthBars.ContainsKey(health)) return;
            
            var newHealthBar = _healthBarPool.Spawn(health);
            newHealthBar.transform.SetParent(transform);
            _healthBars.Add(health, newHealthBar);
        }

        private void RemoveHealthBar(Health health)
        {
            if (!_healthBars.ContainsKey(health)) return;

            _healthBarPool.Despawn(_healthBars[health]);
            _healthBars.Remove(health);
            if (_healthBarPool.NumTotal != 0) return;

            Health.OnHealthAdded -= AddHealthBar;
            Health.OnHealthRemoved -= RemoveHealthBar;
        }
    }
}