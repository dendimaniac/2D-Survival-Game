using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.TestTools;
using Zenject;
using CustomFactories;
using NUnit.Framework;
using SpawnersTypes;
using UnityEngine;

namespace PlaymodeTests
{
    public class EnemySpawnerTest : ZenjectIntegrationTestFixture
    {
        private const BindingFlags PrivateFieldAccessFlags =
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Default;

        private GameObject _spawnerGameObject;
        private EnemySpawner _enemySpawner;

        private void CommonInstall(int count)
        {
            _spawnerGameObject = new GameObject();
            _enemySpawner = _spawnerGameObject.AddComponent<EnemySpawner>();
            InitEnemyList(count);

            PreInstall();

            Container.BindFactory<Object, Vector3, Quaternion, Transform, Transform, TransformFactory>()
                .FromFactory<CustomTransformFactory>();

            PostInstall();
        }

        private void InitEnemyList(int count)
        {
            var enemyList = new List<GameObject>();
            for (var i = 1; i <= count; ++i)
            {
                var enemyGameObject = new GameObject($"Enemy {i}");
                enemyGameObject.AddComponent<TestComponent>();
                enemyList.Add(enemyGameObject);
            }

            SetEnemyListField(enemyList.ToArray());
        }

        private void SetEnemyListField(GameObject[] enemyList)
        {
            var enemyListField =
                _enemySpawner.GetType().GetField("enemyList", PrivateFieldAccessFlags);
            if (enemyListField == null)
            {
                Assert.Fail("Forced failed, might be that enemyList field name changed");
                return;
            }

            enemyListField.SetValue(_enemySpawner, enemyList);
        }

        private static TestComponent[] GetOnlySpawnedEnemies()
        {
            var allTestEnemies = Object.FindObjectsOfType<TestComponent>();
            return allTestEnemies
                .Where(testComponent => testComponent.gameObject.name.ToLower().Contains("clone"))
                .ToArray();
        }

        [UnityTest]
        public IEnumerator Spawn_Only_One_Enemy_When_Only_One_Item_In_Enemy_List()
        {
            CommonInstall(1);

            yield return null;

            var testEnemySpawned = Object.FindObjectOfType<TestComponent>();
            var enemiesSpawned = GetOnlySpawnedEnemies();
            Assert.NotNull(testEnemySpawned);
            Assert.AreEqual(1, enemiesSpawned.Length);
        }

        [UnityTest]
        public IEnumerator Spawn_One_Random_Enemy_When_Enemy_List_Length_Larger_Than_One()
        {
            CommonInstall(2);

            yield return null;

            var testEnemySpawned = Object.FindObjectOfType<TestComponent>();
            var spawnedEnemyName = testEnemySpawned.gameObject.name;
            var enemiesSpawned = GetOnlySpawnedEnemies();
            Debug.Log(spawnedEnemyName);

            Assert.NotNull(testEnemySpawned);
            Assert.AreEqual(1, enemiesSpawned.Length);
            Assert.IsTrue(spawnedEnemyName.Contains("1") || spawnedEnemyName.Contains("2"));
        }

        [UnityTest]
        public IEnumerator Spawn_Another_Random_Enemy_After_Certain_Time()
        {
            CommonInstall(1);

            var timeBetweenSpawnField =
                _enemySpawner.GetType().BaseType?.GetField("timeBetweenSpawn", PrivateFieldAccessFlags);
            if (timeBetweenSpawnField == null)
            {
                Assert.Fail("Forced failed, might be that timeBetweenSpawn field name changed");
            }

            var timeBetweenSpawnValue = timeBetweenSpawnField.GetValue(_enemySpawner);

            yield return null;

            var testEnemySpawned = Object.FindObjectOfType<TestComponent>();
            var enemiesSpawned = GetOnlySpawnedEnemies();

            Assert.NotNull(testEnemySpawned);
            Assert.AreEqual(1, enemiesSpawned.Length);

            yield return new WaitForSeconds((float) timeBetweenSpawnValue);

            enemiesSpawned = GetOnlySpawnedEnemies();
            Assert.AreEqual(2, enemiesSpawned.Length);
        }

        private class TestComponent : MonoBehaviour
        {
        }
    }
}