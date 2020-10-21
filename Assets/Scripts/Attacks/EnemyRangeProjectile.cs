using Helpers;
using Player;
using UnityEngine;
using Zenject;

namespace Attacks
{
    public class EnemyRangeProjectile : RangeProjectile
    {
        private Transform _targetTransform;
        private PlayerInput _playerInput;

        [Inject]
        public void Construct(GameManager gameManager, PlayerInput playerInput)
        {
            _playerInput = playerInput;
        }

        protected override void Awake()
        {
            base.Awake();
            _targetTransform = _playerInput.transform;
        }

        protected override void Update()
        {
            if (!_targetTransform) return;

            base.Update();
        }

        protected override bool CantShoot()
        {
            return !(Time.time >= NextTimeToFire);
        }

        protected override Vector3 GetTargetDirection()
        {
            return (_targetTransform.position - transform.position).normalized;
        }
    }
}