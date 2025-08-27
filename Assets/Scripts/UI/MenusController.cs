using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class MenusController : MonoBehaviour
{
    [SerializeField] private GameObject home;
    [SerializeField] private GameObject credits;
    [SerializeField] private GameObject rank;
    [SerializeField] private GameObject optionsPopUp;
    [SerializeField] private GameObject namePopUp;
    [SerializeField] private GameObject loadPopUp;

    [Header("Rank")]
    [SerializeField] private TMP_InputField playerName;
    [SerializeField] private TextMeshProUGUI warning;
    [SerializeField] private Transform rankGridsViewport;
    [SerializeField] private RankGrid rankGridPrefab;
    [SerializeField] private Color[] rankColors;

    [Header("Game Over")]
    [SerializeField] private GameObject lose;
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI streak;
    [SerializeField] private TextMeshProUGUI stage;
    [SerializeField] private Button resume;
    [SerializeField] private Button stop;
    [SerializeField] private Button back;

    [Header("Options")]
    [SerializeField] private Slider musicMasterVolume;
    [SerializeField] private Slider SFXMasterVolume;

    public Slider MusicVolume => musicMasterVolume;
    public Slider SFXVolume => SFXMasterVolume;

    private ObjectsPool<RankGrid> rankGridPool;

    public EMenu CurrentMenu { get; private set ; }
    public string PlayerName => playerName.text.ToLower();

    private void Awake()
    {
        rankGridPool = new ObjectsPool<RankGrid>(rankGridPrefab);
    }

    private void CloseMenus()
    {
        if (home != null)
        {
            home.SetActive(false);
        }

        if (lose != null)
        {
            lose.SetActive(false);
        }

        if (optionsPopUp != null)
        {
            optionsPopUp.SetActive(false);
        }

        if (credits != null)
        {
            credits.SetActive(false);
        }

        if (rank != null)
        {
            rank.SetActive(false);
        }

        if (namePopUp != null)
        {
            namePopUp.SetActive(false);
        }

        if (loadPopUp != null)
        {
            loadPopUp.SetActive(false);
        }
    }

    public void SetMenu(EMenu menu)
    {
        CloseMenus();
        SetActiveOptionsPopUp(false);

        switch (menu)
        {
            case EMenu.Home:
                if (home != null)
                {
                    home.SetActive(true);
                }
                break;

            case EMenu.Lose:
                if (lose != null)
                {
                    lose.SetActive(true);
                }
                break;

            case EMenu.Credits:
                if (credits != null)
                {
                    credits.SetActive(true);
                }
                break;

            case EMenu.Rank:
                if (rank != null)
                {
                    rank.SetActive(true);
                }
                break;
        }

        CurrentMenu = menu;
    }

    public void SetActiveOptionsPopUp(bool newActive)
    {
        if (optionsPopUp != null)
        {
            optionsPopUp.SetActive(newActive);
        }
    }

    public void SetActiveNamePopUp(bool newActive)
    {
        if (namePopUp != null)
        {
            namePopUp.SetActive(newActive);
        }
    }

    public void SetActiveLoadPopUp(bool newActive)
    {
        if (loadPopUp != null)
        {
            loadPopUp.SetActive(newActive);
        }
    }

    public void SetFinalStats(int score, int streak, int stage)
    {
        this.score.text = score.ToString("000000");
        this.streak.text = "x" + streak.ToString();
        this.stage.text = stage.ToString();
    }

    public void SetPauseButtonsActive(bool newIsActive)
    {
        resume.gameObject.SetActive(newIsActive);
        stop.gameObject.SetActive(newIsActive);
        back.gameObject.SetActive(!newIsActive);
    }

    public void ShowRankGrids(params RankPlayerData[] playerDatas)
    {
        int viewportChildCount = rankGridsViewport.childCount;

        for (int i = viewportChildCount - 1; i > 0; i--)
        {
            GameObject rankGrid = rankGridsViewport.GetChild(i).gameObject;
            rankGrid.SetActive(false);
            rankGrid.transform.SetParent(null);
        }

        for (int i = 0; i < playerDatas.Length; i++)
        {
            RankGrid newRankGrid = rankGridPool.GetObject(rankGridsViewport.position);
            newRankGrid.transform.SetParent(rankGridsViewport);
            newRankGrid.SetValues(playerDatas[i]);
            newRankGrid.SetRank(i + 1);

            if (i < rankColors.Length)
            {
                newRankGrid.SetColor(rankColors[i]);
            }
        }
    }

    public void ShowWarning(int warningID)
    {
        string warningMessage = "";

        switch (warningID)
        {
            case 0:
                warning.text = "";
                return;

            case 1:
                warningMessage = "repeated";
                break;

            case 2:
                warningMessage = "empty";
                break;
        }

        warning.text = "Name\n" + warningMessage + "*";
    }

    public void ClearName() 
    {
        playerName.text = "";
    }
}

public enum EMenu { None, Home, Lose, Options, Credits, Rank, Pause, Name }