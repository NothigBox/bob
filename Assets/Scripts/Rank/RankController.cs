using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class RankController : MonoBehaviour
{
    public bool useLeaderboarKeys;

    [SerializeField] private RankReadWrite readWrite;

    public Action OnLoadRankDatasBegin;
    public Action<bool> OnLoadRankDatasEnd;
    
    public Action OnRankUpdate;
    public Action OnTryToSaveCurrentPlayerData;

    public Action OnDataSaveBegin;

    public Action OnDataSaveEnd 
    { 
        get 
        { 
            return readWrite.OnSavePlayerEnd; 
        }
        set 
        {
            readWrite.OnSavePlayerEnd = value;
        }
    }

    public RankPlayerData CurrentPlayerData { get; private set; }
    public RankPlayerData[] RankPlayerDatas => readWrite.RankPlayerDatas;

    private void Update()
    {
        if(useLeaderboarKeys == false) return;

        if (Input.GetKeyDown(KeyCode.U))
        {
            var dataA = new RankPlayerData("goal", 1500, 12, 4);
            
            readWrite.SaveData(dataA);
        }
    }

    public void LoadRankData(bool doShowRank)
    {
        StopAllCoroutines();
        StartCoroutine(LoadRankDataCoroutine(doShowRank));
    }

    IEnumerator LoadRankDataCoroutine(bool doShowRank)
    {
        OnLoadRankDatasBegin?.Invoke();

        readWrite.LoadData();

        yield return new WaitUntil(() => readWrite.RankPlayerDatas != null);

        OnLoadRankDatasEnd?.Invoke(doShowRank);
    }

    public void RankNewPlayerScore(RankPlayerData playerData)
    {
        readWrite.LoadPlayerData(() => PlayerLoaded(playerData), () => PlayerLoadFailed(playerData));
    }

    public void PlayerLoaded(RankPlayerData playerData)
    {
        bool isInRank = IsScoreInRank(playerData.score);

        if (playerData.score > readWrite.CurrentPlayerData.score && isInRank)
        {
            OnRankUpdate.Invoke();
        }
    }

    public void PlayerLoadFailed(RankPlayerData playerData)
    {
        bool isInRank = IsScoreInRank(playerData.score);

        if (isInRank == true)
        {
            OnRankUpdate?.Invoke();
        }
    }

    private bool IsScoreInRank(int score)
    {
        bool isInRank = false;

        if (RankPlayerDatas.Length > 0)
        {
            for (int i = 0; i < RankPlayerDatas.Length; i++)
            {
                if (score > RankPlayerDatas[i].score)
                {
                    isInRank = true;
                    break;
                }
            }
        }
        else
        {
            isInRank = true;
        }

        return isInRank;
    }

    public bool? SaveCurrentPlayerData(RankPlayerData playerData)
    {
        bool? isNameAvailable = true;

        if (playerData.name == "")
        {
            return null;
        }

        if (isNameAvailable == false)
        {
            return false;
        }

        readWrite.SaveData(playerData);

        OnDataSaveBegin?.Invoke();

        return true;
    }
}
