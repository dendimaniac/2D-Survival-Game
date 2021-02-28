using System.Collections;
using System.Reflection;
using NSubstitute;
using NUnit.Framework;
using PickupsTypes;
using UnityEngine;
using UnityEngine.TestTools;
using Zenject;

namespace PlaymodeTests
{
    public class AmmoPickupsTest : ZenjectIntegrationTestFixture
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
        public IEnumerator
            Picked_Up_Event_Should_Invoke_When_Game_Object_With_Player_Tag_Triggers_With_Ammo_Pickups()
        {
            SetupAmmoPickups();

            var eventRaised = false;
            AmmoPickups.OnAmmoPickedUp += _ => { eventRaised = true; };

            yield return new WaitForFixedUpdate();

            Assert.IsTrue(eventRaised);
        }

        [UnityTest]
        public IEnumerator
            Picked_Up_Event_Should_Not_Invoke_When_Game_Object_Without_Player_Tag_Triggers_With_Ammo_Pickups()
        {
            SetupAmmoPickups();

            _playerGameObject.tag = "Untagged";
            var eventRaised = false;
            AmmoPickups.OnAmmoPickedUp += _ => { eventRaised = true; };

            yield return new WaitForFixedUpdate();

            Assert.IsFalse(eventRaised);
        }

        [UnityTest]
        public IEnumerator
            Picked_Up_Event_Should_Not_Invoke_When_Game_Object_With_Player_Tag_Collides_With_Ammo_Pickups()
        {
            SetupAmmoPickups();

            _ammoPickupsBoxCollider.isTrigger = false;
            var eventRaised = false;
            AmmoPickups.OnAmmoPickedUp += _ => { eventRaised = true; };

            yield return new WaitForFixedUpdate();

            Assert.IsFalse(eventRaised);
        }

        [UnityTest]
        public IEnumerator
            Picked_Up_Event_Should_Invoke_With_Ammo_Amount_When_Game_Object_With_Player_Tag_Triggers_With_Ammo_Pickups()
        {
            SetupAmmoPickups();

            var ammoRefillAmount = 0;
            AmmoPickups.OnAmmoPickedUp += ammoAmount => { ammoRefillAmount = ammoAmount; };

            yield return new WaitForFixedUpdate();

            Assert.IsTrue(ammoRefillAmount > 0f);
        }

        [UnityTest]
        public IEnumerator
            Ammo_Pickups_Is_Despawned_When_Game_Object_With_Player_Tag_Triggers_With_Ammo_Pickups()
        {
            SetupAmmoPickups();

            yield return new WaitForFixedUpdate();

            _memoryPool.Received(1).Despawn(_ammoPickups);
        }

        [UnityTest]
        public IEnumerator Memory_Pool_Is_Null_When_Despawned()
        {
            SetupAmmoPickups();

            _ammoPickups.OnDespawned();

            const BindingFlags privateFieldAccessFlags =
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Default;
            var memoryPoolsProperty = _ammoPickups.GetType().BaseType?.GetField("_memoryPool", privateFieldAccessFlags);
            if (memoryPoolsProperty == null)
            {
                Assert.Fail("Forced failed, might be that _memoryPool field name changed");
            }

            var memoryPoolValue = memoryPoolsProperty.GetValue(_ammoPickups);
            Assert.IsNull(memoryPoolValue);

            yield break;
        }
    }
}