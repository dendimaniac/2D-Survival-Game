﻿using System.Collections;
using PickupsTypes;
using UnityEngine;
using Zenject;

namespace SpawnersTypes
{
    [RequireComponent(typeof(Collider2D))]
    public class PickupsSpawner : Spawner
    {
        [SerializeField] private LayerMask itemBlockingLayer;

        private Collider2D _spawnArea;
        private readonly Collider2D[] _results = new Collider2D[2];
        private int _resultAmount;
        private Vector2 _spawnPosition;
        private Bounds _spawnAreaBounds;
        
        private Pickups.Factory _pickupsFactory;

        [Inject]
        private void Construct(Pickups.Factory pickupsFactory)
        {
            _pickupsFactory = pickupsFactory;
        }
        
        private void Awake()
        {
            _spawnArea = GetComponent<Collider2D>();
            _spawnAreaBounds = _spawnArea.bounds;
        }

        protected override IEnumerator Spawn()
        {
            while (true)
            {
                do
                {
                    var randomXPosition = Random.Range(_spawnAreaBounds.min.x, _spawnAreaBounds.max.x);
                    var randomYPosition = Random.Range(_spawnAreaBounds.min.y, _spawnAreaBounds.max.y);
                    _spawnPosition = new Vector2(randomXPosition, randomYPosition);
                    _resultAmount = Physics2D.OverlapBoxNonAlloc(_spawnPosition, Vector2.one, 0, _results, itemBlockingLayer);
                } while (!_spawnArea.OverlapPoint(_spawnPosition) || _resultAmount > 0);

                var pickupToSpawn = RandomObject();
                pickupToSpawn.transform.position = _spawnPosition;
                pickupToSpawn.SetActive(true);

                yield return new WaitForSeconds(timeBetweenSpawn);
            }
        }
        
        private GameObject RandomObject()
        {
            return _pickupsFactory.Create().gameObject;
        }
    }
}