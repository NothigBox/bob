using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RankGrid : PoolObject
{
    [SerializeField] private new TextMeshProUGUI rank;
    [SerializeField] private new TextMeshProUGUI name;
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI stage;
    [SerializeField] private TextMeshProUGUI streak;

    public void SetValues(string name, int score, int stage, int streak)
    {
        this.name.text = name;
        this.score.text = score.ToString("000000");
        this.stage.text = stage.ToString();
        this.streak.text = "x" + streak.ToString();
    }

    public void SetValues(RankPlayerData playerData)
    {
        SetValues(playerData.name, playerData.score, playerData.stage, playerData.streak);
    }

    public void SetRank(int rank)
    {
        this.rank.text = rank.ToString();
    }

    public void SetColor(Color color)
    {
        rank.color = color;
        name.color = color;
        score.color = color;
        stage.color = color;
        streak.color = color;
    }
}
