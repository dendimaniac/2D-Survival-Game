using Helpers;
using Interfaces;
using PickupsTypes;
using Props;
using Signals;
using UI;
using UnityEngine;
using Zenject;

namespace Player
{
    [RequireComponent(typeof(AttackedAnimatorHandler))]
    public class PlayerHealth : Health, IHealable
    {
        private AttackedAnimatorHandler _animatorHandler;
        private UIManager _uiManager;
        private GameManager _gameManager;
        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus, UIManager uiManager, GameManager gameManager)
        {
            _signalBus = signalBus;
            _uiManager = uiManager;
            _gameManager = gameManager;
        }
        
        private void Awake()
        {
            _animatorHandler = GetComponent<AttackedAnimatorHandler>();
            
            _signalBus.Subscribe<GameLostSignal>(DisableOnDead);
            HealthPickups.OnHealthPickedUp += Heal;
        }

        protected override void Die()
        {
            _uiManager.SetLosingReasonText("You Died!");
            _gameManager.GameLost();
        }

        public override void TakeDamage(int damageAmount)
        {
            _animatorHandler.PlayDamagedAnimation();
            base.TakeDamage(damageAmount);
        }

        private void DisableOnDead()
        {
            GetComponent<Collider2D>().enabled = false;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _signalBus.Unsubscribe<GameLostSignal>(DisableOnDead);
        }

        public void Heal(int healAmount)
        {
            CurrentHealth += healAmount;
        }
    }
}