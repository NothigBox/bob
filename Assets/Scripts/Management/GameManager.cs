using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    const int MIN_STREAK_TO_ADD_LIFE = 5;
    const int STREAK_HEAL = 1;

    [SerializeField] private LifeController life;
    [SerializeField] private UIManager ui;
    [SerializeField] private NeedleManager needle;
    [SerializeField] private ScoreController score;
    [SerializeField] private LevelController level;
    [SerializeField] private new AudioManager audio;
    [SerializeField] private RankController rank;

    [SerializeField] private float timeToEndLevel;

    private bool isGameOver;
    private bool isCurrentlyPaused;
    private bool isSaving;

    private void Awake()
    {
        isCurrentlyPaused = false;

        #region BUBBLES

        Bubble.OnBubblePop += (position, size) => audio.PlayBubblePop();
        BubbleMultiple.OnMultiplePop += (position, size) => audio.PlayBubblePop();
        BubbleSimple.OnBubblePop += (position, size) => audio.PlayBubblePop();

        #endregion

        #region NEEDLE

        needle.OnPop += (score, isBubbleGold) =>
        {
            this.score.AddNewScore(score);
            this.score.AddScore(score, isBubbleGold);

            ui.UpdateNewScore(this.score.CurrentNewScore);
            ui.UpdateBubbleScore(this.score.CurrentBubbleScorePercentage);
            audio.PlayNeedlePop();

            if (isBubbleGold)
            {
                bool didGoldHeal = life.AddLife(2);
                bool didStreakHeal = (needle.CurrentStreak >= MIN_STREAK_TO_ADD_LIFE && needle.CurrentStreak % 2 != 0);

                needle.HealCallback(false, false);
                needle.HealCallback(didStreakHeal && didGoldHeal, didGoldHeal);
            }
        };

        needle.OnStreak += (streak) => 
        { 
            if(streak >= 5 && streak % 2 != 0)
            {
                bool didHeal = life.AddLife(STREAK_HEAL);
                needle.HealCallback(didHeal, false);
            }
            else
            {
                needle.HealCallback(false, false);
            }
        };

        needle.OnStreakTimeEnd += () =>
        {
            ui.UpdateScore(score.CurrentScore);
            score.ResetNewScore();
            needle.ClearStreak();
        };

        needle.OnAbilityShot += () =>
        {
            audio.PlayNeedleAbility();
        };

        #endregion

        #region LIFE

        life.OnLifeUpdate = (currentLife) =>
        {
            ui.UpdateLife(currentLife);
            
            if (currentLife <= 0)
            {
                EndGame();
            }
        };

        life.OnDamaged = () =>
        {
            ui.ActivateDamage();
        };

        #endregion

        #region SCORE

        score.OnBubbleScoreGoalReached = (stage) =>
        {
            level.ActivateBubbleScoreGoalReward();
            ui.UpdateStage(stage);
            ui.SetScoreGoalRewardMessage(life.HurtCount);

            if (stage % 3 == 0)
            {
                needle.AddAbility(5);
            }
            else
            {
                needle.AddAbility(1);
            }
        };

        #endregion

        #region LEVEL

        level.OnBubbleScoreGoalRewardEnd = () =>
        {
            score.ResetBubbleScoreGoal(level.CurrentBubbleScoreGoal);
            ui.UpdateBubbleScore(score.CurrentBubbleScorePercentage, true);
            life.ClearHurtCount();
        };

        level.OnAllBubblesPopped = (score) =>
        {
            this.score.AddNewScore(score);
            this.score.AddScore(score, true);

            ui.UpdateNewScore(this.score.CurrentNewScore);
        };

        level.OnWindSpawned = () => 
        {
            audio.PlayWind();
        };

        #endregion

        #region RANK

        rank.OnLoadRankDatasBegin = () =>
        {
            ui.SetLoadPopUp(true);
        };

        rank.OnDataSaveBegin = () =>
        {
            ui.SetLoadPopUp(true);
        };

        rank.OnLoadRankDatasEnd = (doShowRank) => 
        {
            if (doShowRank == true)
            {
                ui.SetRankGrids(rank.RankPlayerDatas);
            }
            else
            {
                RankPlayerData data = new RankPlayerData("NewPlayerData", score.CurrentScore, needle.HighStreak, score.CurrentStage);

                rank.RankNewPlayerScore(data);
            }

            ui.SetLoadPopUp(false);
        };

        rank.OnRankUpdate = () => 
        {
            CancelInvoke();
            ui.SetMenu(EMenu.Name);
            Debug.Log("***** Can Save Now");
        };

        rank.OnDataSaveEnd = () =>
        {
            isSaving = false;

            ui.SetLoadPopUp(false);
        };

        rank.OnTryToSaveCurrentPlayerData = () =>
        {
            SaveName();
        };

        #endregion

        #region UI

        ui.OnMusicVolumeUpdate = (volume) =>
        {
            audio.UpdateMusicVolume(volume);
        };

        ui.OnSFXVolumeUpdate = (volume) =>
        {
            audio.UpdateSFXVolume(volume);
        };

        ui.OnRewardEnded += (callback) =>
        {
            if (level.ValidateIsWindNextLevel())
            {
                callback.Invoke();
            }
        };

        #endregion
    }

    private void Start()
    {
        GoHome(true);

        ResetGameStats();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (ui.CurrentMenu == EMenu.None)
            {
                Pause(!isCurrentlyPaused);
            }
            else if (ui.CurrentMenu == EMenu.Home)
            {
                if(ui.CurrentMenu != EMenu.Options)
                {
                    GoOptions();
                }
                else
                {
                    GoHome(false);
                }
            }
        }
    }

    public void StartGame()
    {
        ui.SetMenu(EMenu.None);
        level.SetNewLevel(0);
        audio.SetMusic(EMusic.Initial, true);
        needle.ActivateAbilityDelay(0.5f);
        Pause(false);

        ResetGameStats();
    }

    public void GoHome(bool forceMusic)
    {
        Time.timeScale = 1f;

        level.Stop();
        ui.SetMenu(EMenu.Home);
        audio.SetMusic(EMusic.Menu, forceMusic);

        EndGame(false);
    }

    public void GoCredits()
    {
        level.Stop();
        ui.SetMenu(EMenu.Credits);
    }

    public void GoOptions()
    {
        ui.SetMenu(EMenu.Options);
    }

    public void Pause(bool newPause)
    {
        if (isGameOver == true) return;

        isCurrentlyPaused = newPause;

        if (isCurrentlyPaused)
        {
            ui.SetMenu(EMenu.Pause);
            level.Stop();
            needle.SetAbilityActive(false);

            Time.timeScale = 0;
        }
        else
        {
            ui.SetMenu(EMenu.None);
            level.Resume();
            needle.ActivateAbilityDelay(0.5f);

            Time.timeScale = 1;
        }
    }
    
    public void GoRank()
    {
        ui.SetMenu(EMenu.Rank);
        rank.LoadRankData(true);
    }

    private void EndGame(bool showEndPanel = true)
    {
        isGameOver = true;
        level.Stop();
        level.KillRestantBubbles();
        ui.SetLifeState(true);
        needle.SetAbilityActive(false);

        if (showEndPanel) 
        {
            rank.LoadRankData(false);

            Invoke(nameof(ShowEndPanel), timeToEndLevel);
        }
    }

    private void ShowEndPanel()
    {
        ui.SetLoseMenu(score.CurrentScore, needle.HighStreak, score.CurrentStage);
    }

    private void ResetGameStats()
    {
        isGameOver = false;

        score.ResetBubbleScoreGoal(level.CurrentBubbleScoreGoal);
        score.ResetStats();
        needle.ResetNeedle();
        life.SetMaxLife();
        life.ClearHurtCount();

        ui.UpdateBubbleScore(0);
        ui.UpdateScore(0);
        ui.UpdateStage(0);
        ui.SetLifeState(false);
    }

    public void SaveName()
    {
        if(isSaving) return;

        //  Try to save player's name
        RankPlayerData playerRank = new RankPlayerData(ui.PlayerName, score.CurrentScore, needle.HighStreak, score.CurrentStage);
        bool? isNameAvailable = rank.SaveCurrentPlayerData(playerRank);

        if (isNameAvailable == false) 
        {
            ui.SetNameWarning(1);

            return;
        }
        else if (isNameAvailable == null)
        {
            ui.SetNameWarning(2);

            return;
        }
        
        isSaving = true;

        ui.SetNameWarning(0);

        Invoke(nameof(ShowEndPanel), 1f);
    }
}
