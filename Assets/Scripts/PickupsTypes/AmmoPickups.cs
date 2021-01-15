using System;
using UnityEngine;

namespace PickupsTypes
{
    public class AmmoPickups : Pickups
    {
        public static event Action<int> OnAmmoPickedUp;

        [SerializeField] private int ammoAmount = 10;

        protected override void OnPickedUp()
        {
            base.OnPickedUp();
            OnAmmoPickedUp?.Invoke(ammoAmount);
        }
    }
}