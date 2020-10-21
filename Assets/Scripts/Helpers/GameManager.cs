﻿using Data;
using Player;
using Signals;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Helpers
{
    public class GameManager : MonoBehaviour
    {
        public Camera mainCamera;

        private SignalBus _signalBus;
        private PlayerInput _playerInput;

        [Inject]
        private void Construct(SignalBus signalBus, PlayerInput playerInput)
        {
            _signalBus = signalBus;
            _playerInput = playerInput;
        }

        private void Awake()
        {
            Time.timeScale = 1;
        }

        public void GameLost()
        {
            _playerInput.enabled = false;
            Time.timeScale = 0;
            _signalBus.Fire<GameLostSignal>();
        }
    }
}