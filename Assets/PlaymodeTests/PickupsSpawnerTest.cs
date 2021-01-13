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
            
            itemBlockingLayerField.SetValue(_pickupsSpawner, unwalkableLayerMask);
        }
        
        private void CommonInstall()
        {
            SetupPickupsSpawner();

            PreInstall();

            Container.BindFactory<Pickups, Pickups.Factory>()
                .FromMonoPoolableMemoryPool(binder =>
                    binder.WithInitialSize(1).To<AmmoPickups>().FromNewComponentOn(_spawnerGameObject)).NonLazy();

            PostInstall();
        }

        [UnityTest]
        public IEnumerator Pickups_Spawner_Create_Pickups_Within_Collider()
        {
            CommonInstall();
            
            yield return null;

            Assert.Fail();
        }
    }
}