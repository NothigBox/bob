using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    private int currentStage;
    private int currentScore;
    private int currentNewScore;
    private int currentBubbleScore;
    [SerializeField] private int bubbleScoreGoal;
    private bool canAddBubbleScore;


    public Action<int> OnBubbleScoreGoalReached;

    public int CurrentBubbleScorePercentage => Mathf.RoundToInt((currentBubbleScore / (float) bubbleScoreGoal) * 100);
    public int CurrentScore => currentScore;
    public int CurrentNewScore => currentNewScore;
    public int CurrentStage => currentStage;

    private void Awake()
    {
        ResetStats();
    }

    public int AddScore(int score, bool isGoldenBubble)
    {
        currentScore += score;

        if (!isGoldenBubble && canAddBubbleScore)
        {
            currentBubbleScore += score;
            ValidateBubbleScore();
        }

        return currentScore;
    }

    public int AddNewScore(int score)
    {
        currentNewScore += score;

        return currentNewScore;
    }

    public void ResetNewScore()
    {
        currentNewScore = 0;
    }

    public void ResetBubbleScoreGoal(int bubbleScoreGoal)
    {
        currentBubbleScore = 0;
        canAddBubbleScore= true;
        this.bubbleScoreGoal = bubbleScoreGoal;
    }

    private void ValidateBubbleScore()
    {
        if (currentBubbleScore >= bubbleScoreGoal && canAddBubbleScore)
        {
            currentStage++;
            canAddBubbleScore = false;
            OnBubbleScoreGoalReached?.Invoke(currentStage);
        }
    }

    public void ResetStats()
    {
        currentStage = 0;
        currentScore = 0;
        currentNewScore = 0;
        currentBubbleScore = 0;
        canAddBubbleScore = true;
    }
}
