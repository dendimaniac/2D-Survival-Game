using System.Collections;
using Helpers;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Props
{
    public class HealthBar : MonoBehaviour
    {
        #region SerializeFields

        [SerializeField] private Image foregroundImage;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private float updateSpeedInSec = 0.5f;
        [SerializeField] private float positionOffset = 1f;
        [SerializeField] private float fadeSeconds = 0.1f;

        #endregion

        private Health _health;
        private GameManager _gameManager;

        [Inject]
        public void Construct(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        private void Awake()
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
            var worldToScreenPoint = _gameManager.mainCamera
                .WorldToScreenPoint(_health.transform.position + positionOffset * Vector3.up);
            transform.position = worldToScreenPoint;
        }

        private void OnDisable()
        {
            _health.OnHealthPctChanged -= HandleHealthChanged;
        }
        
        public class Pool : MemoryPool<Health, HealthBar>
        {
            protected override void OnDespawned(HealthBar item)
            {
                if (!item) return;
                item.gameObject.SetActive(false);
            }

            protected override void Reinitialize(Health health, HealthBar item)
            {
                item.gameObject.SetActive(true);
                item.ResetHealthBar();
                item.SetHealth(health);
            }
        }
    }
}