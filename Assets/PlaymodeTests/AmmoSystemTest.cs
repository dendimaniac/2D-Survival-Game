using System.Collections;
using System.Reflection;
using Attacks;
using NSubstitute;
using NUnit.Framework;
using PickupsTypes;
using UnityEngine;
using UnityEngine.TestTools;
using Zenject;

namespace PlaymodeTests
{
    public class AmmoSystemTest : ZenjectIntegrationTestFixture
    {
        #region AmmoPickups

        private AmmoPickups _ammoPickups;
        private GameObject _ammoPickupsGameObject;
        private BoxCollider2D _ammoPickupsBoxCollider;
        private IMemoryPool _memoryPool;

        #endregion

        #region Player

        private GameObject _playerGameObject;

        #endregion

        private const BindingFlags PrivateFieldAccessFlags =
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Default;

        private void SetupAmmoPickups()
        {
            SetupAmmoPickupsGameObject();

            SetupPlayerGameObject();

            SkipInstall();
        }
    
        private void SetupPlayerGameObject()
        {
            _playerGameObject = new GameObject {tag = "Player"};
            _playerGameObject.AddComponent<Rigidbody2D>();
            _playerGameObject.AddComponent<BoxCollider2D>();
        }

        private void SetupAmmoPickupsGameObject()
        {
            _ammoPickupsGameObject = new GameObject();
            _ammoPickupsBoxCollider = _ammoPickupsGameObject.AddComponent<BoxCollider2D>();
            _ammoPickupsBoxCollider.isTrigger = true;
            _ammoPickups = _ammoPickupsGameObject.AddComponent<AmmoPickups>();
            _memoryPool = Substitute.For<IMemoryPool>();
            _ammoPickups.OnSpawned(_memoryPool);
        }

        [UnityTest]
        public IEnumerator OnAmmoPickedUpEvent_CurrentMaxAmmoRefilledByPickupRefillAmount()
        {
            SetupAmmoPickups();

            const int maxAmmo = 30;
            const int maxAmmoPerClip = 10;
            var ammo = new Ammo(maxAmmo, maxAmmoPerClip);
            ammo.ReduceCurrentAmmo(maxAmmoPerClip);
            ammo.Reload();
            ammo.ReduceCurrentAmmo(maxAmmoPerClip);
            ammo.Reload();
            ammo.ReduceCurrentAmmo(maxAmmoPerClip);
            ammo.Reload();
            var reloadedCurrentAmmo = ammo.CurrentAmmo;
        
            Assert.AreEqual(maxAmmo - maxAmmoPerClip * 3, ammo.CurrentMaxAmmo);
            Assert.AreEqual(maxAmmoPerClip, reloadedCurrentAmmo);

            yield return new WaitForFixedUpdate();
        
            var pickupRefillAmount = _ammoPickups.GetType().GetField("ammoAmount", PrivateFieldAccessFlags);
            if (pickupRefillAmount == null)
            {
                Assert.Fail("Forced failed, might be that ammoAmount field name changed");
            }
            Assert.AreEqual(pickupRefillAmount.GetValue(_ammoPickups), ammo.CurrentMaxAmmo);
            Assert.AreEqual(maxAmmoPerClip, reloadedCurrentAmmo);
        }
        
        [UnityTest]
        public IEnumerator OnAmmoPickedUpEvent_RaiseOnAmmoChangeEvent()
        {
            SetupAmmoPickups();

            const int maxAmmo = 30;
            const int maxAmmoPerClip = 10;
            var ammo = new Ammo(maxAmmo, maxAmmoPerClip);
            ammo.ReduceCurrentAmmo(maxAmmoPerClip);
            ammo.Reload();

            var eventRaised = false;
            Ammo.OnAmmoChanged += _ => { eventRaised = true; };

            yield return new WaitForFixedUpdate();
            
            Assert.IsTrue(eventRaised);
        }

        [UnityTest]
        public IEnumerator OnAmmoPickedUpEvent_AmmoDisposed_DontRefillAmmo()
        {
            SetupAmmoPickups();

            const int maxAmmo = 30;
            const int maxAmmoPerClip = 10;
            var ammo = new Ammo(maxAmmo, maxAmmoPerClip);
            ammo.ReduceCurrentAmmo(maxAmmoPerClip);
            ammo.Reload();
            var reloadedCurrentAmmo = ammo.CurrentAmmo;
        
            Assert.AreEqual(maxAmmo - maxAmmoPerClip, ammo.CurrentMaxAmmo);
            Assert.AreEqual(maxAmmoPerClip, reloadedCurrentAmmo);
            
            ammo.Dispose();

            yield return new WaitForFixedUpdate();
        
            Assert.AreEqual(maxAmmo - maxAmmoPerClip, ammo.CurrentMaxAmmo);
            Assert.AreEqual(maxAmmoPerClip, reloadedCurrentAmmo);
        }
    }
}