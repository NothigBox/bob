using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RankPlayerData
{
    public string name;
    public int score;
    public int streak;
    public int stage;

    public RankPlayerData(string name, int score, int streak, int stage)
    {
        this.name = name;
        this.score = score;
        this.streak = streak;
        this.stage = stage;
    }
}
