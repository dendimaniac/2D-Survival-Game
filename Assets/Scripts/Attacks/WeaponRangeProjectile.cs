using Helpers;
using Player;
using UnityEngine;
using Zenject;

namespace Attacks
{
    public class WeaponRangeProjectile : RangeProjectile
    {
        private GameManager _gameManager;
        private PlayerInput _playerInput;

        [Inject]
        public void Construct(GameManager gameManager, PlayerInput playerInput)
        {
            _gameManager = gameManager;
            _playerInput = playerInput;
        }
        
        protected override void Awake()
        {
            MuzzleTransform = transform.GetChild(0).GetComponent<Transform>();
        }

        protected override bool CantShoot()
        {
            return !_playerInput.CanShoot || !(Time.time >= NextTimeToFire);
        }

        protected override Vector3 GetTargetDirection()
        {
            return (_gameManager.mainCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position)
                .normalized;
        }
    }
}