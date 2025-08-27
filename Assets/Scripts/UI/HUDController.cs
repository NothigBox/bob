using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class HUDController : MonoBehaviour
{
    [SerializeField] private Slider life;
    [SerializeField] private Slider bubbleScore;
    [SerializeField] private Image bubbleScoreFill;
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI newScore;
    [SerializeField] private TextMeshProUGUI stage;
    [SerializeField] private TextMeshProUGUI scoreGoalReward;
    [SerializeField] private Animation hurt;
    [SerializeField] private Animation scoreGoalRewardMessage;
    [SerializeField] private Animation windsIncomingMessage;
    [SerializeField] private Image lifeHandler;
    [SerializeField] private Image lifeBackground;
    [SerializeField] private Color lifeHandlerColor;
    [SerializeField] private Color lifeBackgroundColor;

    public Action<Action> OnScoreGoalRewardMessageEnded;

    private bool isBubbleScoreActive;
    private Color initialLifeHandlerColor;
    private Color initialLifeBackgroundColor;

    [SerializeField] private Color bubbleScoreGoalReached;

    private Color bubbleScoreFillInitial;

    private void Awake()
    {
        bubbleScoreFillInitial = bubbleScoreFill.color;
        initialLifeHandlerColor = lifeHandler.color;
        initialLifeBackgroundColor = lifeBackground.color;
    }

    public void UpdateLife(int currentLife)
    {
        life.value = currentLife;
    }

    public void UpdateScore(int currentScore)
    {
        score.text = currentScore.ToString("000000");
    }

    public void UpdateNewScore(int currentNewScore)
    {
        newScore.text = "+" + currentNewScore.ToString();
    }

    public void ClearNewScore()
    {
        newScore.text = "";
    }

    public void UpdateBubbleScore(int currentBubbleScore, bool doDeactivateScoreGoalReward = false)
    {
        if (doDeactivateScoreGoalReward == true)
        {
            isBubbleScoreActive = false;
        }

        bubbleScore.value = currentBubbleScore;

        if (isBubbleScoreActive == false)
        {
            bubbleScoreFill.color = bubbleScoreFillInitial;
        }
    }

    public void SetScoreGoalReached(bool isBubbleScoreActive)
    {
        this.isBubbleScoreActive = isBubbleScoreActive;

        if (isBubbleScoreActive == true)
        {
            bubbleScoreFill.color = bubbleScoreGoalReached;
        }
    }

    public void TriggerDamageMetaEffect()
    {
        hurt.Play();
    }

    public void UpdateStage(int stage)
    {
        this.stage.text = stage.ToString();
    }

    public void SetLifeEmpty(bool newIsEmpty)
    {
        if (newIsEmpty)
        {
            lifeHandler.color = lifeHandlerColor;
            lifeBackground.color = lifeBackgroundColor;
        }
        else
        {
            lifeHandler.color = initialLifeHandlerColor;
            lifeBackground.color = initialLifeBackgroundColor;
        }
    }

    public void SetScoreGoalReward(int goalReward)
    {
        string rewardMessage = "";

        switch (goalReward) 
        {
            case 0:
                rewardMessage = "Perfect!";
                break;

            case 1:
                rewardMessage = "Great!";
                break;

            case 2:
                rewardMessage = "Good!";
                break;

            case 3:
                rewardMessage = "Continue!";
                break;

            case 4:
                rewardMessage = "Don't give up!";
                break;
        }

        scoreGoalReward.text = rewardMessage;
        scoreGoalRewardMessage.Play();

        SetScoreGoalReached(true);

        Invoke(nameof(CallbackScoreGoalRewardMessageEnd), scoreGoalRewardMessage.clip.length);
    }

    public void CallbackScoreGoalRewardMessageEnd()
    {
        OnScoreGoalRewardMessageEnded?.Invoke(() => ShowWindsIncomingMessage());
    }

    public void ShowWindsIncomingMessage()
    {
        windsIncomingMessage.Play();
    }
}
