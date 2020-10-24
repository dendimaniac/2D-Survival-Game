using Helpers;
using Player;
using UnityEngine;
using Zenject;

namespace Attacks
{
    public class WeaponRangeProjectile : RangeProjectile
    {
        private Camera _mainCamera;
        private PlayerInput _playerInput;

        [Inject]
        public void Construct(Camera mainCamera, PlayerInput playerInput)
        {
            _mainCamera = mainCamera;
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
            return (_mainCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position)
                .normalized;
        }
    }
}