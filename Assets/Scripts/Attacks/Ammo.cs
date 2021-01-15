using System;
using Data;
using PickupsTypes;
using UnityEngine;

namespace Attacks
{
    public class Ammo : IDisposable
    {
        public static event Action<Ammo> OnAmmoChanged;

        public bool IsReloading;

        private readonly int _maxAmmo;
        private readonly int _maxAmmoPerClip;

        private int _currentAmmo;
        private int _currentMaxAmmo;

        public int CurrentMaxAmmo
        {
            get => _currentMaxAmmo;
            private set => _currentMaxAmmo = Mathf.Clamp(value, 0, _maxAmmo);
        }

        public int CurrentAmmo
        {
            get => _currentAmmo;
            private set => _currentAmmo = Mathf.Clamp(value, 0, _maxAmmoPerClip);
        }


        public Ammo(int maxAmmo, int maxAmmoPerClip)
        {
            AmmoPickups.OnAmmoPickedUp += RefillAmmo;

            _maxAmmo = maxAmmo;
            _maxAmmoPerClip = maxAmmoPerClip;
            CurrentMaxAmmo = maxAmmo;
            CurrentAmmo = maxAmmoPerClip;
            OnAmmoChanged?.Invoke(this);
        }

        public void Reload()
        {
            var reloadAmount = GetReloadAmount();
            ResetCurrentAmmo(reloadAmount);
            ReduceCurrentMaxAmmo(reloadAmount);
            IsReloading = false;
            OnAmmoChanged?.Invoke(this);
        }

        public void ReduceCurrentAmmo()
        {
            CurrentAmmo--;
            OnAmmoChanged?.Invoke(this);
        }

        private void ReduceCurrentMaxAmmo(int reduceAmount)
        {
            CurrentMaxAmmo -= reduceAmount;
        }

        private void ResetCurrentAmmo(int reloadAmount)
        {
            CurrentAmmo += reloadAmount;
        }

        public bool IsAmmoEmpty()
        {
            return CurrentMaxAmmo <= 0 && CurrentAmmo <= 0;
        }

        private void RefillAmmo(int refillAmount)
        {
            CurrentMaxAmmo += refillAmount;
            OnAmmoChanged?.Invoke(this);
        }

        private int GetReloadAmount()
        {
            return CurrentMaxAmmo <= (_maxAmmoPerClip - CurrentAmmo) ? CurrentMaxAmmo : (_maxAmmoPerClip - CurrentAmmo);
        }

        public void Dispose()
        {
            AmmoPickups.OnAmmoPickedUp -= RefillAmmo;
        }
    }
}