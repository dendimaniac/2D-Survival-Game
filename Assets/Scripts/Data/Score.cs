using System;
using Enemy;
using Helpers;
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

        public HitCombo HitCombo { get; }
        public int CurrentScore
        {
            get => _currentScore;
            private set
            {
                _currentScore = Mathf.Clamp(value, 0, int.MaxValue);
                OnScoreChanged?.Invoke(CurrentScore);
            }
        }

        public Score(SignalBus signalBus, HitCombo hitCombo)
        {
            _signalBus = signalBus;
            HitCombo = hitCombo;
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
            HitCombo.ResetStreak();
        }

        public void IncreaseScore()
        {
            HitCombo.IncreaseStreak();
            CurrentScore += ScoreIncrement * HitCombo.CurrentHitCombo;
        }

        private void SaveScore()
        {
            var currentHighScore = PlayerPrefs.GetInt(Highscore);
            if (currentHighScore >= CurrentScore) return;
            PlayerPrefs.SetInt(Highscore, CurrentScore);
        }

        public static int LoadHighScore()
        {
            return PlayerPrefs.GetInt(Highscore);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<GameLostSignal>(SaveScore);
        }
    }
}