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
    public class HealthPickupsTest : ZenjectIntegrationTestFixture
    {
        #region HealthPickups

        private HealthPickups _healthPickups;
        private GameObject _healthPickupsGameObject;
        private BoxCollider2D _healthPickupsBoxCollider;
        private IMemoryPool _memoryPool;

        #endregion

        #region Player

        private GameObject _playerGameObject;

        #endregion

        private void SetupHealthPickups()
        {
            SetupHealthPickupsGameObject();

            SetupPlayerGameObject();

            SkipInstall();
        }

        [UnityTest]
        public IEnumerator
            Picked_Up_Event_Should_Invoke_When_Game_Object_With_Player_Tag_Triggers_With_Health_Pickups()
        {
            SetupHealthPickups();

            var eventRaised = false;
            HealthPickups.OnHealthPickedUp += _ => { eventRaised = true; };

            yield return new WaitForFixedUpdate();

            Assert.IsTrue(eventRaised);
        }

        [UnityTest]
        public IEnumerator
            Picked_Up_Event_Should_Not_Invoke_When_Game_Object_Without_Player_Tag_Triggers_With_Health_Pickups()
        {
            SetupHealthPickups();

            _playerGameObject.tag = "Untagged";
            var eventRaised = false;
            HealthPickups.OnHealthPickedUp += _ => { eventRaised = true; };

            yield return new WaitForFixedUpdate();

            Assert.IsFalse(eventRaised);
        }

        [UnityTest]
        public IEnumerator
            Picked_Up_Event_Should_Not_Invoke_When_Game_Object_With_Player_Tag_Collides_With_Health_Pickups()
        {
            SetupHealthPickups();

            _healthPickupsBoxCollider.isTrigger = false;
            var eventRaised = false;
            HealthPickups.OnHealthPickedUp += _ => { eventRaised = true; };

            yield return new WaitForFixedUpdate();

            Assert.IsFalse(eventRaised);
        }

        [UnityTest]
        public IEnumerator
            Picked_Up_Event_Should_Invoke_With_Heal_Amount_When_Game_Object_With_Player_Tag_Triggers_With_Health_Pickups()
        {
            SetupHealthPickups();

            var healthRefillAmount = 0;
            HealthPickups.OnHealthPickedUp += healAmount => { healthRefillAmount = healAmount; };

            yield return new WaitForFixedUpdate();

            Assert.IsTrue(healthRefillAmount > 0f);
        }
        
        [UnityTest]
        public IEnumerator
            Health_Pickups_Is_Despawned_When_Game_Object_With_Player_Tag_Triggers_With_Health_Pickups()
        {
            SetupHealthPickups();

            yield return new WaitForFixedUpdate();
            
            _memoryPool.Received(1).Despawn(_healthPickups);
        }
        
        [UnityTest]
        public IEnumerator Memory_Pool_Is_Null_When_Despawned()
        {
            SetupHealthPickups();

            _healthPickups.OnDespawned();

            const BindingFlags privateFieldAccessFlags =
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Default;
            var memoryPoolsProperty = _healthPickups.GetType().BaseType?.GetField("_memoryPool", privateFieldAccessFlags);
            if (memoryPoolsProperty == null)
            {
                Assert.Fail("Forced failed, might be that _memoryPool field name changed");
            }

            var memoryPoolValue = memoryPoolsProperty.GetValue(_healthPickups);
            Assert.IsNull(memoryPoolValue);

            yield break;
        }

        private void SetupPlayerGameObject()
        {
            _playerGameObject = new GameObject {tag = "Player"};
            _playerGameObject.AddComponent<Rigidbody2D>();
            _playerGameObject.AddComponent<BoxCollider2D>();
        }

        private void SetupHealthPickupsGameObject()
        {
            _healthPickupsGameObject = new GameObject();
            _healthPickupsBoxCollider = _healthPickupsGameObject.AddComponent<BoxCollider2D>();
            _healthPickupsBoxCollider.isTrigger = true;
            _healthPickups = _healthPickupsGameObject.AddComponent<HealthPickups>();
            _memoryPool = Substitute.For<IMemoryPool>();
            _healthPickups.OnSpawned(_memoryPool);
        }
    }
}