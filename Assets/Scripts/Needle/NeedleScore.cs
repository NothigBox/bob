using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NeedleScore : MonoBehaviour
{
    private const int MIN_STREAK = 3;
    private const int STREAK_FRACTION = 3;

    [SerializeField] private float streakDuration;

    private float timerStreakDuration;
    private int currentStreak;

    public Action OnStreakEnd;
    public Action OnStreakTimeEnd;
    public Action<int> OnStreak;
    public Action<int, bool> OnPop;

    private bool IsOnStreak => currentStreak >= MIN_STREAK;

    public int CurrentStreak => currentStreak;
    public int HighStreak { get; private set; }
    public float StreakDuration => streakDuration;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.GetContact(0).normal == Vector2.down)
        {
            Debug.DrawRay(transform.position, Vector2.up, Color.cyan, 2f);

            Bubble bubble = collision.transform.GetComponent<Bubble>();
            int score = bubble.Pop();

            AddScore(score, bubble is BubbleGold);
        }
    }

    private void FixedUpdate()
    {
        if (timerStreakDuration >= streakDuration)
        {
            if (IsOnStreak)
            {
                OnStreakEnd?.Invoke();
            }

            currentStreak = 0;

            OnStreakTimeEnd?.Invoke();
        }

        timerStreakDuration += Time.fixedDeltaTime;
    }

    private void ValidateStreak()
    {
        timerStreakDuration = 0f;

        if(IsOnStreak)
        {
            OnStreak?.Invoke(currentStreak);
        }
    }

    public void ResetStats()
    {
        timerStreakDuration = 0f;
        currentStreak = 0;
        HighStreak = 0;
    }

    public void AddScore(int score, bool isBubbleGold)
    {
        if (score != default)
        {
            if (IsOnStreak)
            {
                score *= currentStreak / STREAK_FRACTION;
            }

            currentStreak++;

            if (currentStreak > HighStreak)
            {
                HighStreak = currentStreak;
            }

            ValidateStreak();

            OnPop?.Invoke(score, isBubbleGold);
        }
    }
}
