using Helpers;
using Player;
using UnityEngine;
using Zenject;

namespace Attacks
{
    public class EnemyRangeHitScan : RangeHitScan
    {
        private Transform _playerTransform;
        private PlayerInput _playerInput;

        [Inject]
        public void Construct(GameManager gameManager, PlayerInput playerInput)
        {
            _playerInput = playerInput;
        }

        protected override void Awake()
        {
            MuzzleTransform = transform;
            _playerTransform = _playerInput.transform;
        }

        protected override void Update()
        {
            if (!_playerTransform) return;

            base.Update();
        }

        protected override bool CantShoot()
        {
            return !(Time.time >= NextTimeToFire);
        }

        protected override bool CheckHit(out RaycastHit2D hitInfo)
        {
            Vector2 position = MuzzleTransform.position;
            var targetDirection = (Vector2) _playerTransform.position - position;
            hitInfo = Physics2D.Raycast(position, targetDirection, shootingRange, possibleHitLayer);
            return !hitInfo;
        }
    }
}