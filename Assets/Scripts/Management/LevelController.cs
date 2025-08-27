using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    const float WAIT_BUBBLE_SCORE_GOAL_REWARD = 2.2f;
    const float WAIT_WINDS = 6f;
    const int LEVELS_FOR_WIND = 5;

    [SerializeField] private WindSpawner windSpawner;
    [SerializeField] private BubbleSpawner bubbleSpawner;
    [SerializeField] private NeedleArea needleArea;

    [SerializeField] private LevelInfo[] levels;

    private int bubbleScoreGoal;
    [SerializeField] private float timeBetweenBubbles;
    [SerializeField] private float timeBetweenWinds;
    private float bubbleScoreGoalDuration;

    private float timerTimeBetweenBubbles;
    private float timerWinds;
    private float timerTimeBetweenWinds;

    private bool isBubbleScoreGoalRewardActive;
    private bool canSpawnBubbles;
    private bool canSpawnWinds;

    private int currentLevel;

    public Action OnBubbleScoreGoalRewardEnd;
    public Action OnWindSpawned;

    public Action<int> OnAllBubblesPopped
    {
        get
        {
            return bubbleSpawner.OnAllBubblesPopped;
        }
        set
        {
            bubbleSpawner.OnAllBubblesPopped = value;
        }
    }

    private LevelInfo CurrentLevel => levels[currentLevel];

    public int CurrentBubbleScoreGoal => CurrentLevel.BubbleScoreGoal;

    private void Awake()
    {
        currentLevel = 0;

        timeBetweenBubbles = CurrentLevel.TimeBetweenBubbles;

        canSpawnBubbles = false;
        canSpawnWinds = false;
        isBubbleScoreGoalRewardActive = false;
    }

    private void FixedUpdate()
    {
        if(canSpawnBubbles == false) return;

        if (timerTimeBetweenBubbles >= timeBetweenBubbles)
        {
            if (isBubbleScoreGoalRewardActive == true)
            {
                bubbleSpawner.SpawnGoldBubblesAtRandomPoints(CurrentLevel.GoldBubbleSpawnCount);
            }
            else 
            {
                bubbleSpawner.SpawnBubblesAtRandomPoints(CurrentLevel.BubbleSpawnCount);
            }

            timerTimeBetweenBubbles = 0f;
        }

        if (canSpawnWinds == true)
        {
            if (ValidateIsWindLevel(currentLevel) && timerTimeBetweenWinds >= timeBetweenWinds)
            {
                int windForceMultiplier = (int) Mathf.Floor(currentLevel / LEVELS_FOR_WIND) - 1;

                if(windForceMultiplier < 0)
                {
                    windForceMultiplier = 0;
                }

                windSpawner.SpawnWindsAtRandomPoints(windForceMultiplier);

                timerTimeBetweenWinds = 0f;

                OnWindSpawned?.Invoke();
            }
        }

        float fixedDeltaTime = Time.fixedDeltaTime;

        timerTimeBetweenWinds += fixedDeltaTime;
        timerTimeBetweenBubbles += fixedDeltaTime;
    }

    public void SetNewLevel(int index)
    {
        currentLevel = index;
        canSpawnBubbles = true;
        needleArea.canMoveNeedle = true;
        timerTimeBetweenBubbles = 0f;
        timeBetweenBubbles = CurrentLevel.TimeBetweenBubbles;

        bubbleSpawner.SetProbabilities(CurrentLevel.Probabilities);
    }

    public void GoToNextLevel()
    {
        ++currentLevel;

        if (currentLevel >= levels.Length)
        {
            currentLevel = levels.Length - 1;
        }

        SetNewLevel(currentLevel);
    }

    public void ActivateBubbleScoreGoalReward()
    {
        StopAllCoroutines();
        StartCoroutine(BubbleScoreGoalRewardCoroutine());
    }

    IEnumerator BubbleScoreGoalRewardCoroutine() 
    {
        int initialLevel = currentLevel;
        float initialTimeBetweenBubbles = timeBetweenBubbles;

        canSpawnBubbles = false;
        canSpawnWinds = false;

        bubbleSpawner.KillAllBubbles(true);

        yield return new WaitForSeconds(WAIT_BUBBLE_SCORE_GOAL_REWARD);

        timeBetweenBubbles = 1f;
        timerTimeBetweenBubbles = timeBetweenBubbles;
        canSpawnBubbles = true;
        isBubbleScoreGoalRewardActive = true;

        yield return new WaitForSeconds(CurrentLevel.BubbleScoreGoalDuration);

        canSpawnBubbles = false;
        isBubbleScoreGoalRewardActive = false;
        timeBetweenBubbles = initialTimeBetweenBubbles;

        yield return new WaitForSeconds(WAIT_BUBBLE_SCORE_GOAL_REWARD * 3f);

        GoToNextLevel();
        timerTimeBetweenBubbles = 0f;
        canSpawnBubbles = true;

        OnBubbleScoreGoalRewardEnd?.Invoke();

        yield return new WaitForSeconds(WAIT_WINDS);

        timerTimeBetweenWinds = 0f;
        canSpawnWinds = true;

        //Debug.Log("Winds can spawn now");

        yield return null;
    }

    public void KillRestantBubbles()
    {
        bubbleSpawner.KillAllBubbles(false);
    }

    public void Stop()
    {
        canSpawnBubbles = false;
        needleArea.canMoveNeedle = false;
    }

    public void Resume()
    {
        canSpawnBubbles = true;
        needleArea.canMoveNeedle = true;
    }

    private bool ValidateIsWindLevel(int level)
    {
        bool result = false;

        bool isHigherThanZero = level > 0;

        bool isWindLevel = level % LEVELS_FOR_WIND == 0;

        result = isHigherThanZero && isWindLevel;

        return result; 
    }

    public bool ValidateIsWindNextLevel()
    {
        return ValidateIsWindLevel(currentLevel + 1);
    }
}
