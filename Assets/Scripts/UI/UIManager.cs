using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    const float WARNING_DURATION = 2f;

    [SerializeField] private HUDController hud;
    [SerializeField] private MenusController menus;

    public Action<float> OnMusicVolumeUpdate;
    public Action<float> OnSFXVolumeUpdate;

    public Action<Action> OnRewardEnded
    {
        get
        {
            return hud.OnScoreGoalRewardMessageEnded;
        }

        set
        {
            hud.OnScoreGoalRewardMessageEnded = value;
        }
    }

    public EMenu CurrentMenu => menus.CurrentMenu;

    public string PlayerName => menus.PlayerName;

    public void SetMenu(EMenu menu)
    {
        if (menu == EMenu.Pause)
        {
            menus.SetMenu(EMenu.None);

            menus.SetActiveOptionsPopUp(true);
            menus.SetPauseButtonsActive(true);

            return;
        }
        else if (menu == EMenu.Options)
        {
            menus.SetActiveOptionsPopUp(true);
            menus.SetPauseButtonsActive(false);

            return;
        }
        else if (menu == EMenu.Name)
        {
            ClearNameWarning();
            menus.ClearName();
            menus.SetActiveNamePopUp(true);

            return;
        }

        menus.SetMenu(menu);
    }

    public void SetLoseMenu(int score, int streak, int stage)
    {
        SetMenu(EMenu.Lose);
        menus.SetFinalStats(score, streak, stage);
    }

    public void UpdateScore(int currentScore)
    {
        hud.UpdateScore(currentScore);
        hud.ClearNewScore();
    }

    public void UpdateNewScore(int newScore)
    {
        hud.UpdateNewScore(newScore);
    }

    public void UpdateBubbleScore(int currentBubbleScorePercentage, bool doDeactivateScoreGoalReward = false)
    {
        hud.UpdateBubbleScore(currentBubbleScorePercentage, doDeactivateScoreGoalReward);
    }

    public void UpdateLife(int currentLife) 
    {
        hud.UpdateLife(currentLife);
    }

    public void ActivateDamage()
    {
        hud.TriggerDamageMetaEffect();
    }

    public void UpdateStage(int stage)
    {
        hud.UpdateStage(stage);
    }

    public void SetLifeState(bool isEmpty)
    {
        hud.SetLifeEmpty(isEmpty);
    }

    public void SetScoreGoalRewardMessage(int goalReward)
    {
        hud.SetScoreGoalReward(goalReward);
    }

    public void SetLoadPopUp(bool newActive)
    {
        menus.SetActiveLoadPopUp(newActive);
    }

    public void SetRankGrids(params RankPlayerData[] playerDatas)
    {
        menus.ShowRankGrids(playerDatas);
    }

    public void CloseNamePopUp()
    {
        menus.SetActiveNamePopUp(false);
    }

    public void SetNameWarning(int warningID)
    {
        menus.ShowWarning(warningID);

        Invoke(nameof(ClearNameWarning), WARNING_DURATION);
    }

    public void ClearNameWarning()
    {
        menus.ShowWarning(0);
    }

    public void MusicVolumeUpdate()
    {
        OnMusicVolumeUpdate?.Invoke(menus.MusicVolume.value);
    }

    public void SFXVolumeUpdate()
    {
        OnSFXVolumeUpdate?.Invoke(menus.SFXVolume.value);
    }
}
