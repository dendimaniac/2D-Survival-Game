using Helpers;
using Interfaces;
using UnityEngine;
using Zenject;

namespace Player
{
    [RequireComponent(typeof(PlayerMovement))]
    public class PlayerInput : MonoBehaviour
    {
        public bool CanShoot { get; private set; }
        public float HorizontalMovement { get; private set; }
        public float VerticalMovement { get; private set; }
        public Vector2 RotateDirection { get; private set; }

        private bool _readyToClear;
        private Camera _mainCamera;

        [Inject]
        private void Construct(Camera mainCamera)
        {
            _mainCamera = mainCamera;
        }

        private void Update()
        {
            ClearInput();

            ProcessInputs();

            HorizontalMovement = Mathf.Clamp(HorizontalMovement, -1f, 1f);
            VerticalMovement = Mathf.Clamp(VerticalMovement, -1f, 1f);
        }

        private void ClearInput()
        {
            if (!_readyToClear)
                return;

            HorizontalMovement = 0f;
            VerticalMovement = 0f;
            RotateDirection = Vector2.zero;
            CanShoot = false;

            _readyToClear = false;
        }

        private void ProcessInputs()
        {
            HorizontalMovement += Input.GetAxisRaw("Horizontal");
            VerticalMovement += Input.GetAxisRaw("Vertical");

            CanShoot = CanShoot || Input.GetButton("Fire1");

            Vector2 mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RotateDirection = (mousePosition - (Vector2) transform.position).normalized;
        }

        private void FixedUpdate()
        {
            _readyToClear = true;
        }
    }
}