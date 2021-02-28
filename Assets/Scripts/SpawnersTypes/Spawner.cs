using System.Collections;
using UnityEngine;

namespace SpawnersTypes
{
    public abstract class Spawner : MonoBehaviour
    {
        [SerializeField] protected float timeBetweenSpawn;

        private void Start()
        {
            StartCoroutine(Spawn());
        }

        protected abstract IEnumerator Spawn();
    }
}