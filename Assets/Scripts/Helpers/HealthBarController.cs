using System.Collections.Generic;
using Props;
using UnityEngine;
using Zenject;

namespace Helpers
{
    public class HealthBarController : MonoBehaviour
    {
        private readonly Dictionary<Health, HealthBar> _healthBars = new Dictionary<Health, HealthBar>();
        private HealthBar.Factory _healthBarFactory;

        [Inject]
        public void Construct(HealthBar.Factory healthBarFactory)
        {
            _healthBarFactory = healthBarFactory;
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
            
            var newHealthBar = _healthBarFactory.Create(health);
            newHealthBar.transform.SetParent(transform);
            _healthBars.Add(health, newHealthBar);
        }

        private void RemoveHealthBar(Health health)
        {
            if (!_healthBars.ContainsKey(health)) return;

            if (_healthBars[health] != null)
            {
                _healthBars[health].Dispose();
            }
            _healthBars.Remove(health);
            if (_healthBars.Count != 0) return;

            Health.OnHealthAdded -= AddHealthBar;
            Health.OnHealthRemoved -= RemoveHealthBar;
        }
    }
}