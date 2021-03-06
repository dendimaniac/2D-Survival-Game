﻿using System;
using Enemy;
using Helpers;
using Interfaces;
using Signals;
using UnityEngine;
using Zenject;

namespace Data
{
    public class Score: IDisposable, IInitializable
    {
        public static event Action<int> OnScoreChanged;
        public int ScoreIncrement = 10;
        
        private const string Highscore = "highscore";
        private int _currentScore;
        
        private readonly SignalBus _signalBus;
        private readonly HitCombo _hitCombo;
        private readonly IPlayerPrefs _playerPrefs;
        
        public int CurrentScore
        {
            get => _currentScore;
            private set
            {
                _currentScore = Mathf.Clamp(value, 0, int.MaxValue);
                OnScoreChanged?.Invoke(CurrentScore);
            }
        }

        public Score(SignalBus signalBus, HitCombo hitCombo, IPlayerPrefs playerPrefs)
        {
            _signalBus = signalBus;
            _hitCombo = hitCombo;
            _playerPrefs = playerPrefs;
        }
        
        public void Initialize()
        {
            ResetScore();
            _signalBus.Subscribe<GameLostSignal>(SaveScore);
            EnemyHealth.OnEnemyHit += IncreaseScore;
        }

        public void ResetScore()
        {
            CurrentScore = 0;
            _hitCombo.ResetStreak();
        }

        public void IncreaseScore()
        {
            _hitCombo.IncreaseStreak();
            CurrentScore += ScoreIncrement * _hitCombo.CurrentHitCombo;
        }

        private void SaveScore()
        {
            var currentHighScore = _playerPrefs.GetInt(Highscore);
            if (currentHighScore >= CurrentScore) return;
            _playerPrefs.SetInt(Highscore, CurrentScore);
        }

        public int LoadHighScore()
        {
            return _playerPrefs.GetInt(Highscore);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<GameLostSignal>(SaveScore);
        }
    }
}