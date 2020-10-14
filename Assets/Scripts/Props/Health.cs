using System;
using Interfaces;
using UnityEngine;

namespace Props
{
    [RequireComponent(typeof(Animator))]
    public abstract class Health : MonoBehaviour, IDamageable
    {
        #region ExposedFields

        public static event Action<Health> OnHealthAdded = delegate { };
        public static event Action<Health> OnHealthRemoved = delegate { };
        public event Action<float> OnHealthPctChanged;

        [SerializeField] protected int maxHealth = 100;

        #endregion

        #region NonExposedFields

        private int _currentHealth;

        protected int CurrentHealth
        {
            get => _currentHealth;
            set
            {
                _currentHealth = value;
                _currentHealth = Mathf.Clamp(_currentHealth, 0, maxHealth);

                var currentHealthPct = (float) _currentHealth / maxHealth;
                OnHealthPctChanged?.Invoke(currentHealthPct);
            }
        }

        #endregion

        private void Start()
        {
            CurrentHealth = maxHealth;
            OnHealthAdded(this);
        }

        public virtual void TakeDamage(int damageAmount)
        {
            CurrentHealth -= damageAmount;
            if (CurrentHealth <= 0)
            {
                Die();
            }
        }

        protected abstract void Die();

        protected virtual void OnDisable()
        {
            OnHealthRemoved(this);
        }
    }
}