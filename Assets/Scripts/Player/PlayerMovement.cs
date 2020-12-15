using System;
using Helpers;
using Interfaces;
using Signals;
using UnityEngine;
using Zenject;

namespace Player
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerMovement : MonoBehaviour
    {
        #region SerializeFields

        [SerializeField] private float speed;
        [SerializeField] private Transform weaponsHolder;

        #endregion

        #region NonSerializeFields

        private Vector2 _movement;
        private Transform _playerTransform;
        private SpriteRenderer _spriteRenderer;
        private PlayerInput _playerInput;
        private SignalBus _signalBus;

        #endregion

        [Inject]
        private void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        private void Awake()
        {
            _playerTransform = transform;

            _spriteRenderer = GetComponent<SpriteRenderer>();
            _playerInput = GetComponent<PlayerInput>();

            _signalBus.Subscribe<GameLostSignal>(DisableOnDead);
        }

        private void FixedUpdate()
        {
            CheckFlip(_playerInput.HorizontalMovement);
            Move(_playerInput.HorizontalMovement, _playerInput.VerticalMovement);
            Rotate(_playerInput.RotateDirection);
        }

        private void CheckFlip(float h)
        {
            if (Math.Abs(h) < Mathf.Epsilon) return;
            _spriteRenderer.flipX = h < 0;
        }

        private void Move(float h, float v)
        {
            _movement.Set(h, v);
            _movement = Time.deltaTime * speed * _movement.normalized;
            _playerTransform.position += (Vector3) _movement;
        }

        private void Rotate(Vector2 rotateDirection)
        {
            var angle = Mathf.Atan2(rotateDirection.y, rotateDirection.x) * Mathf.Rad2Deg;
            weaponsHolder.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        private void DisableOnDead()
        {
            GetComponent<PlayerMovement>().enabled = false;
            GetComponent<Animator>().enabled = false;
        }

        private void OnDisable()
        {
            _signalBus.Unsubscribe<GameLostSignal>(DisableOnDead);
        }
    }
}