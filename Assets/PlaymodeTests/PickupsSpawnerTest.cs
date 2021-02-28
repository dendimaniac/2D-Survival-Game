using System.Collections;
using System.Reflection;
using NUnit.Framework;
using PickupsTypes;
using SpawnersTypes;
using UnityEngine;
using UnityEngine.TestTools;
using Zenject;

namespace PlaymodeTests
{
    public class PickupsSpawnerTest : ZenjectIntegrationTestFixture
    {
        private GameObject _spawnerGameObject;
        private BoxCollider2D _spawnerBoxCollider;
        private PickupsSpawner _pickupsSpawner;

        private void SetupPickupsSpawner()
        {
            _spawnerGameObject = new GameObject();
            _spawnerBoxCollider = _spawnerGameObject.AddComponent<BoxCollider2D>();
            _spawnerBoxCollider.isTrigger = true;
            _pickupsSpawner = _spawnerGameObject.AddComponent<PickupsSpawner>();
            SetItemBlockLayerField(LayerMask.NameToLayer("Unwalkable"));
        }

        private void SetItemBlockLayerField(LayerMask unwalkableLayerMask)
        {
            const BindingFlags privateFieldAccessFlags =
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Default;
            var itemBlockingLayerField =
                _pickupsSpawner.GetType().GetField("itemBlockingLayer", privateFieldAccessFlags);
            if (itemBlockingLayerField == null)
            {
                Assert.Fail("Forced failed, might be that itemBlockingLayer field name changed");
                return;
            }

            LayerMask itemBlockingLayer = 1 << unwalkableLayerMask;
            itemBlockingLayerField.SetValue(_pickupsSpawner, itemBlockingLayer);
        }

        private void CommonInstall()
        {
            SetupPickupsSpawner();
            var pickups = new GameObject();
            pickups.AddComponent<BoxCollider2D>();

            PreInstall();

            Container.BindFactory<Pickups, Pickups.Factory>().To<AmmoPickups>().FromNewComponentOn(pickups);

            PostInstall();
        }
        
        private void InstallWithUnwalkableCollider(Vector2 spawnerBoxColliderSize, out BoxCollider2D unwalkableCollider)
        {
            var unwalkableLayerMask = LayerMask.NameToLayer("Unwalkable");
            _spawnerGameObject = new GameObject("Pickups Spawner");
            _spawnerBoxCollider = _spawnerGameObject.AddComponent<BoxCollider2D>();
            _spawnerBoxCollider.size = spawnerBoxColliderSize;
            _spawnerBoxCollider.isTrigger = true;
            _pickupsSpawner = _spawnerGameObject.AddComponent<PickupsSpawner>();
            SetItemBlockLayerField(unwalkableLayerMask);

            var unwalkableGameObject = new GameObject("Unwalkable Layer") {layer = unwalkableLayerMask};
            unwalkableCollider = unwalkableGameObject.AddComponent<BoxCollider2D>();

            var pickups = new GameObject("Factory");
            pickups.AddComponent<BoxCollider2D>();

            PreInstall();

            Container.BindFactory<Pickups, Pickups.Factory>().To<AmmoPickups>().FromNewComponentOn(pickups);

            PostInstall();
        }
        
        private static void AssertPickupsSpawnedWithinSpawnerCollider(Vector3 spawnedPickups, Bounds spawnerColliderBounds)
        {
            Assert.IsTrue(spawnedPickups.x >= spawnerColliderBounds.min.x);
            Assert.IsTrue(spawnedPickups.x <= spawnerColliderBounds.max.x);
            Assert.IsTrue(spawnedPickups.y >= spawnerColliderBounds.min.y);
            Assert.IsTrue(spawnedPickups.y <= spawnerColliderBounds.max.y);
        }

        [UnityTest]
        public IEnumerator Pickups_Spawner_Create_Pickups_Within_Collider()
        {
            CommonInstall();

            yield return null;

            var spawnedPickups = Object.FindObjectOfType<Pickups>().transform.position;
            var spawnerColliderBounds = _spawnerBoxCollider.bounds;

            AssertPickupsSpawnedWithinSpawnerCollider(spawnedPickups, spawnerColliderBounds);
        }

        [UnityTest]
        public IEnumerator Pickups_Spawner_Create_Many_Pickups_Within_Collider_After_Certain_Time()
        {
            CommonInstall();

            yield return null;

            const BindingFlags privateFieldAccessFlags =
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Default;
            var timeBetweenSpawnField =
                _pickupsSpawner.GetType().BaseType?.GetField("timeBetweenSpawn", privateFieldAccessFlags);
            if (timeBetweenSpawnField == null)
            {
                Assert.Fail("Forced failed, might be that _memoryPool field name changed");
            }

            var timeBetweenSpawnValue = timeBetweenSpawnField.GetValue(_pickupsSpawner);
            var spawnedPickupsList = Object.FindObjectsOfType<Pickups>();
            Assert.AreEqual(1, spawnedPickupsList.Length);

            yield return new WaitForSeconds((float) timeBetweenSpawnValue);

            spawnedPickupsList = Object.FindObjectsOfType<Pickups>();
            Assert.AreEqual(2, spawnedPickupsList.Length);

            var spawnerColliderBounds = _spawnerBoxCollider.bounds;
            foreach (var pickups in spawnedPickupsList)
            {
                var position = pickups.transform.position;
                AssertPickupsSpawnedWithinSpawnerCollider(position, spawnerColliderBounds);
            }
        }

        [UnityTest]
        public IEnumerator Pickups_Spawner_Avoid_Unwalkable_Layer_Mask_X_Axis()
        {
            InstallWithUnwalkableCollider(new Vector2(5f, 1f), out var unwalkableCollider);

            yield return new WaitForFixedUpdate();

            var spawnedPickups = Object.FindObjectOfType<Pickups>();
            var spawnedPickupsPosition = spawnedPickups.transform.position;
            var spawnedPickupsCollier = spawnedPickups.GetComponent<BoxCollider2D>();
            var spawnerColliderBounds = _spawnerBoxCollider.bounds;

            AssertPickupsSpawnedWithinSpawnerCollider(spawnedPickupsPosition, spawnerColliderBounds);

            if (spawnedPickupsPosition.x > 0)
            {
                Assert.IsTrue(spawnedPickupsPosition.x - spawnedPickupsCollier.bounds.extents.x >=
                              unwalkableCollider.bounds.max.x);
            }
            else
            {
                Assert.IsTrue(spawnedPickupsPosition.x + spawnedPickupsCollier.bounds.extents.x <=
                              unwalkableCollider.bounds.min.x);
            }
        }
        
        [UnityTest]
        public IEnumerator Pickups_Spawner_Avoid_Unwalkable_Layer_Mask_Y_Axis()
        {
            InstallWithUnwalkableCollider(new Vector2(1f, 5f), out var unwalkableCollider);

            yield return new WaitForFixedUpdate();

            var spawnedPickups = Object.FindObjectOfType<Pickups>();
            var spawnedPickupsPosition = spawnedPickups.transform.position;
            var spawnedPickupsCollier = spawnedPickups.GetComponent<BoxCollider2D>();
            var spawnerColliderBounds = _spawnerBoxCollider.bounds;

            AssertPickupsSpawnedWithinSpawnerCollider(spawnedPickupsPosition, spawnerColliderBounds);

            if (spawnedPickupsPosition.y > 0)
            {
                Assert.IsTrue(spawnedPickupsPosition.y - spawnedPickupsCollier.bounds.extents.y >=
                              unwalkableCollider.bounds.max.y);
            }
            else
            {
                Assert.IsTrue(spawnedPickupsPosition.y + spawnedPickupsCollier.bounds.extents.y <=
                              unwalkableCollider.bounds.min.y);
            }
        }
    }
}