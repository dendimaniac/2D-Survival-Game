using System;
using Helpers;
using Interfaces;
using UnityEngine;

namespace Props
{
    [RequireComponent(typeof(Animator))]
    public abstract class Health : MonoBehaviour, IDamageable
    {
        public static event Action<Health> OnHealthAdded;
        public static event Action<Health> OnHealthRemoved;
        public event Action<float> OnHealthPctChanged;

        [SerializeField] protected int maxHealth = 100;

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

        private int _currentHealth;

        private void Start()
        {
            CurrentHealth = maxHealth;
            OnHealthAdded?.Invoke(this);
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
            OnHealthRemoved?.Invoke(this);
        }
    }
}