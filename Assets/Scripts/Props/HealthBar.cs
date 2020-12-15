using System;
using System.Collections;
using Helpers;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Props
{
    public class HealthBar : MonoBehaviour, IPoolable<Health, IMemoryPool>, IDisposable
    {
        #region SerializeFields

        [SerializeField] private Image foregroundImage;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private float updateSpeedInSec = 0.5f;
        [SerializeField] private float positionOffset = 1f;
        [SerializeField] private float fadeSeconds = 0.1f;

        #endregion

        private Health _health;
        private Camera _mainCamera;
        private IMemoryPool _memoryPool;

        [Inject]
        public void Construct(Camera mainCamera)
        {
            _mainCamera = mainCamera;
        }

        private void OnEnable()
        {
            FadeHealthBar(0, 0);
        }

        private void ResetHealthBar()
        {
            foregroundImage.fillAmount = 1f;
        }

        private void SetHealth(Health healthToSet)
        {
            _health = healthToSet;
            healthToSet.OnHealthPctChanged += HandleHealthChanged;
        }

        private void HandleHealthChanged(float pct)
        {
            FadeHealthBar(1, fadeSeconds);

            StopAllCoroutines();
            StartCoroutine(ChangeToPct(pct));
        }

        private void FadeHealthBar(float alphaValue, float seconds)
        {
            foregroundImage.CrossFadeAlpha(alphaValue, seconds, false);
            backgroundImage.CrossFadeAlpha(alphaValue, seconds, false);
        }

        private IEnumerator ChangeToPct(float pct)
        {
            var preChangedPct = foregroundImage.fillAmount;
            var elapsedTime = 0f;

            while (elapsedTime < updateSpeedInSec)
            {
                elapsedTime += Time.deltaTime;
                foregroundImage.fillAmount = Mathf.Lerp(preChangedPct, pct, elapsedTime / updateSpeedInSec);
                yield return null;
            }

            foregroundImage.fillAmount = pct;
        }

        private void LateUpdate()
        {
            var worldToScreenPoint = _mainCamera
                .WorldToScreenPoint(_health.transform.position + positionOffset * Vector3.up);
            transform.position = worldToScreenPoint;
        }

        public void OnDespawned()
        {
            _memoryPool = null;
        }

        public void OnSpawned(Health health, IMemoryPool memoryPool)
        {
            _memoryPool = memoryPool;
            ResetHealthBar();
            SetHealth(health);
        }

        public void Dispose()
        {
            _memoryPool.Despawn(this);
        }

        private void OnDestroy()
        {
            _health.OnHealthPctChanged -= HandleHealthChanged;
        }

        public class Factory : PlaceholderFactory<Health, HealthBar>
        {
        }
    }
}