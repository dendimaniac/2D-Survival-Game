﻿using System;
using UnityEngine;

namespace PickupsTypes
{
    public class HealthPickups : Pickups
    {
        public static event Action<int> OnHealthPickedUp;

        [SerializeField] private int healAmount = 10;

        protected override void OnPickedUp()
        {
            OnHealthPickedUp?.Invoke(healAmount);
            Destroy(gameObject);
        }
    }
}