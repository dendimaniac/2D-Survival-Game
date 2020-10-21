using System.Collections;
using Player;
using UnityEngine;

namespace Attacks
{
    public class WeaponRangeHitScan : RangeHitScan
    {
        [SerializeField] private bool useAmmo;
        [SerializeField] private int maxAmmo = 30;
        [SerializeField] private int maxAmmoPerClip = 10;
        [SerializeField] private float reloadTime = 1f;
        [SerializeField] private PlayerInput playerInput;

        private Ammo _ammo;

        private void OnValidate()
        {
            maxAmmo = Mathf.Clamp(maxAmmo, 0, 70);
            maxAmmoPerClip = Mathf.Clamp(maxAmmoPerClip, 0, maxAmmo);
        }

        protected override void Awake()
        {
            base.Awake();
            if (useAmmo)
            {
                _ammo = new Ammo(maxAmmo, maxAmmoPerClip);
            }

            MuzzleTransform = transform.GetChild(0).GetComponent<Transform>();
        }

        protected override void Shoot()
        {
            if (useAmmo)
            {
                if (_ammo.CurrentAmmo <= 0 && !_ammo.IsReloading)
                {
                    StartCoroutine(ReloadAmmo());
                    return;
                }

                _ammo.ReduceCurrentAmmo();
            }

            base.Shoot();
        }

        private IEnumerator ReloadAmmo()
        {
            _ammo.IsReloading = true;
            yield return new WaitForSeconds(reloadTime);
            _ammo.Reload();
        }

        protected override bool CantShoot()
        {
            if (useAmmo)
            {
                return !playerInput.CanShoot || !(Time.time >= NextTimeToFire) || _ammo.IsAmmoEmpty() ||
                       _ammo.IsReloading;
            }

            return !playerInput.CanShoot || !(Time.time >= NextTimeToFire);
        }

        private void OnDisable()
        {
            BulletLineRenderer.enabled = false;

            if (useAmmo)
            {
                _ammo.IsReloading = false;
            }
        }
    }
}