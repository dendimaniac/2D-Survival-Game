using Helpers;
using Player;
using Signals;
using UnityEngine;
using Zenject;

namespace Attacks
{
    [RequireComponent(typeof(MeleeWeaponAnimatorHandler))]
    public class MeleeWeapon : MeleeAttack
    {
        [SerializeField] private PlayerInput playerInput;
        
        private MeleeWeaponAnimatorHandler _animatorHandler;
        private SignalBus _signalBus;

        [Inject]
        private void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }
        
        protected override void Awake()
        {
            _animatorHandler = GetComponent<MeleeWeaponAnimatorHandler>();
            _signalBus.Subscribe<GameLostSignal>(DisableOnDead);
            base.Awake();
        }

        protected override void Update()
        {
            base.Update();

            if (CantDamage()) return;

            _animatorHandler.PlayAttackAnimation();
            Attack();
        }

        protected virtual bool CantDamage()
        {
            return !playerInput.CanShoot || !(Timer >= timeBetweenAttack);
        }

        private void DisableOnDead()
        {
            GetComponent<MeleeWeapon>().enabled = false;
            GetComponent<Animator>().enabled = false;
        }

        private void OnDisable()
        {
            Timer = timeBetweenAttack;
        }

        private void OnDestroy()
        {
            _signalBus.Unsubscribe<GameLostSignal>(DisableOnDead);
        }
    }
}