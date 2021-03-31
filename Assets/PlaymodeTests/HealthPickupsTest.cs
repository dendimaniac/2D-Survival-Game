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
        public IEnumerator OnHealthPickedUpEvent_GameObjectWithPlayerTagTriggers_IsRaised()
        {
            SetupHealthPickups();

            var eventRaised = false;
            HealthPickups.OnHealthPickedUp += _ => { eventRaised = true; };

            yield return new WaitForFixedUpdate();

            Assert.IsTrue(eventRaised);
        }

        [UnityTest]
        public IEnumerator OnHealthPickedUpEvent_GameObjectWithoutPlayerTagTriggers_NotRaised()
        {
            SetupHealthPickups();

            _playerGameObject.tag = "Untagged";
            var eventRaised = false;
            HealthPickups.OnHealthPickedUp += _ => { eventRaised = true; };

            yield return new WaitForFixedUpdate();

            Assert.IsFalse(eventRaised);
        }

        [UnityTest]
        public IEnumerator OnHealthPickedUpEvent_GameObjectWithPlayerTagCollides_NotRaised()
        {
            SetupHealthPickups();

            _healthPickupsBoxCollider.isTrigger = false;
            var eventRaised = false;
            HealthPickups.OnHealthPickedUp += _ => { eventRaised = true; };

            yield return new WaitForFixedUpdate();

            Assert.IsFalse(eventRaised);
        }
        
        [UnityTest]
        public IEnumerator OnHealthPickedUpEvent_GameObjectWithPlayerTagTriggers_RaisedWithHealAmount()
        {
            SetupHealthPickups();

            var healthRefillAmount = 0;
            HealthPickups.OnHealthPickedUp += healAmount => { healthRefillAmount = healAmount; };

            yield return new WaitForFixedUpdate();

            Assert.IsTrue(healthRefillAmount > 0f);
        }
        
        [UnityTest]
        public IEnumerator Despawn_GameObjectWithPlayerTagTriggers_IsCalled()
        {
            SetupHealthPickups();

            yield return new WaitForFixedUpdate();

            _memoryPool.Received(1).Despawn(_healthPickups);
        }
        
        [UnityTest]
        public IEnumerator MemoryPool_DespawnCalled_ReturnsNull()
        {
            SetupHealthPickups();

            _healthPickups.OnDespawned();

            const BindingFlags privateFieldAccessFlags =
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Default;
            var memoryPoolsProperty =
                _healthPickups.GetType().BaseType?.GetField("_memoryPool", privateFieldAccessFlags);
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