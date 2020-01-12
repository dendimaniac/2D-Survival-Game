﻿using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreSystemUI : MonoBehaviour
{
    #region SerializeFields

    public static ScoreSystemUI instance;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private Slider comboTimerSlider;
    [SerializeField] private float maxResetTime = 3;

    public int scoreIncrement;

    #endregion

    #region NonSerializeFields

    private float currentTimer;
    private Animator comboTextAnimator;
    private static readonly int Hit = Animator.StringToHash("Hit");
    private static readonly int ShouldFlick = Animator.StringToHash("ShouldFlick");
    private static readonly int FlickSpeed = Animator.StringToHash("FlickSpeed");
    private static readonly int EndCombo = Animator.StringToHash("EndCombo");

    #endregion

    private void Awake()
    {
        if (instance != null)
            return;
        instance = this;

        comboTextAnimator = comboText.GetComponent<Animator>();

        comboTimerSlider.maxValue = maxResetTime;
        
        Score.ResetScore();
        HitCombo.Instance.ResetStreak();

        EnemyHealth.EnemyHit += HitCombo.Instance.IncreaseStreak;
        EnemyHealth.EnemyHit += Score.IncreaseScore;
        EnemyHealth.EnemyHit += UpdateComboText;
        EnemyHealth.EnemyHit += UpdateScoreText;
        EnemyHealth.EnemyHit += TimerCoroutineController;
    }

    private void UpdateComboText()
    {
        comboText.text = $"x{HitCombo.Instance.hitCombo}";
        comboTextAnimator.SetTrigger(Hit);
    }

    private void UpdateScoreText()
    {
        scoreText.text = Score.currentScore.ToString();
    }

    private void UpdateComboTimerText()
    {
        comboTimerSlider.value = currentTimer;
    }

    private void TimerCoroutineController()
    {
        StopAllCoroutines();
        StartCoroutine(ComboTimer());
    }

    private IEnumerator ComboTimer()
    {
        currentTimer = maxResetTime;
        comboTextAnimator.SetBool(ShouldFlick, false);
        comboTextAnimator.SetFloat(FlickSpeed, 0);
        while (currentTimer > 0)
        {
            UpdateComboTimerText();
            currentTimer -= Time.deltaTime;
            if (currentTimer <= maxResetTime / 2)
            {
                comboTextAnimator.SetBool(ShouldFlick, true);
                comboTextAnimator.SetFloat(FlickSpeed, maxResetTime / currentTimer);
            }
            yield return null;
        }
        OnComboEnd();
    }

    private void OnComboEnd()
    {
        comboTextAnimator.SetBool(EndCombo, true);
        StartCoroutine(UIManager.Instance.FadeLoseMenu());
        GameManager.Instance.GameLost();
    }
}