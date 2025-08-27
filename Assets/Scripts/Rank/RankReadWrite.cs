using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using DeleteOptions = Unity.Services.CloudSave.Models.Data.Player.DeleteOptions;

public class RankReadWrite : MonoBehaviour
{
    const string LEADERBOARD_ID = "Rank_bob";

    private RankPlayerData[] rankPlayerDatas;
    public RankPlayerData[] RankPlayerDatas => rankPlayerDatas;

    private RankPlayerData currentPlayerData;
    public RankPlayerData CurrentPlayerData => currentPlayerData;

    public Action OnSavePlayerEnd;
    public Action OnLoadRankEnd;
    public Action OnLoadPlayerEnd;
    public Action OnLoadPlayerFailed;

    private void Start()
    {
        SetUpAndSignIn();
    }

    async void SetUpAndSignIn()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void SaveData(RankPlayerData player)
    {
        await LeaderboardsService.Instance.AddPlayerScoreAsync(LEADERBOARD_ID, player.score, new AddPlayerScoreOptions { Metadata = player });

        OnSavePlayerEnd?.Invoke();
    }

    public async void LoadData()
    {
        List<RankPlayerData> data = new List<RankPlayerData>();
        rankPlayerDatas = null;

        var scoreResponse = await LeaderboardsService.Instance
        .GetScoresAsync(
            LEADERBOARD_ID,
            new GetScoresOptions { Limit = 6, IncludeMetadata = true }
        );

        for (int i = 0; i < scoreResponse.Results.Count; i++)
        {
            RankPlayerData rankPD = JsonUtility.FromJson<RankPlayerData>(scoreResponse.Results[i].Metadata);
            data.Add(rankPD);

            Debug.Log(scoreResponse.Results[i].Metadata);
        }

        rankPlayerDatas = data.ToArray();

        OnLoadRankEnd?.Invoke();
    }

    public async void LoadPlayerData(Action successCallback, Action failCallback)
    {
        currentPlayerData = null;

        try
        {
            var scoreResponse = await LeaderboardsService.Instance
            .GetPlayerScoreAsync(
                LEADERBOARD_ID,
                new GetPlayerScoreOptions { IncludeMetadata = true }
            );

            currentPlayerData = JsonUtility.FromJson<RankPlayerData>(scoreResponse.Metadata);

            successCallback?.Invoke();

            OnLoadPlayerEnd?.Invoke();
        }
        catch 
        {
            failCallback?.Invoke();

            OnLoadPlayerFailed?.Invoke();
        }
    }
}
