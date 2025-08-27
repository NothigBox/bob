using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct LevelInfo
{
    [SerializeField] private int bubbleScoreGoal;

    [Header("Bubbles")]
    [SerializeField] private int bubbleSpawnCount;
    [SerializeField] private float timeBetweenBubbles;

    [Header("Gold Bubbles")]
    [SerializeField] private int goldBubbleSpawnCount;
    [SerializeField] private float bubbleScoreGoalDuration;

    [SerializeField] private float[] probabilities;

    public int BubbleSpawnCount => bubbleSpawnCount;
    public int GoldBubbleSpawnCount => goldBubbleSpawnCount;
    public int BubbleScoreGoal => bubbleScoreGoal;

    public float TimeBetweenBubbles => timeBetweenBubbles;
    public float BubbleScoreGoalDuration => bubbleScoreGoalDuration;

    public float[] Probabilities => probabilities;
}
