using System;
using UnityEngine;
using Zenject;

namespace PickupsTypes
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class Pickups : MonoBehaviour, IPoolable<IMemoryPool>, IDisposable
    {
        private IMemoryPool _memoryPool;
        
        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            OnPickedUp();
        }

        protected virtual void OnPickedUp()
        {
            Dispose();
        }
        
        public virtual void OnDespawned()
        {
            _memoryPool = null;
        }

        public virtual void OnSpawned(IMemoryPool memoryPool)
        {
            _memoryPool = memoryPool;
        }

        public virtual void Dispose()
        {
            _memoryPool.Despawn(this);
        }
        
        public class Factory : PlaceholderFactory<Pickups>
        {
            
        }
    }
}