using System.Collections;
using CustomFactories;
using UnityEngine;
using Zenject;

namespace SpawnersTypes
{
    public class EnemySpawner : Spawner
    {
        [SerializeField] private GameObject[] enemyList;

        private TransformFactory _transformFactory;

        [Inject]
        private void Construct(TransformFactory transformFactory)
        {
            _transformFactory = transformFactory;
        }

        protected override IEnumerator Spawn()
        {
            while (true)
            {
                var spawnerTransform = transform;
                var enemyToSpawn = RandomObject();

                _transformFactory.Create(enemyToSpawn, spawnerTransform.position,
                    enemyToSpawn.transform.rotation, spawnerTransform);
                yield return new WaitForSeconds(timeBetweenSpawn);
            }
        }

        private GameObject RandomObject()
        {
            return enemyList[RandomIndex()];
        }

        private int RandomIndex()
        {
            return Random.Range(0, enemyList.Length);
        }
    }
}