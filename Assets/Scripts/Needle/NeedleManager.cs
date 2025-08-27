using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class NeedleManager : MonoBehaviour
{
    private NeedleScore score;
    private NeedleMovement movement;
    private NeedleUI ui;
    private NeedleAbility ability;
    private bool canUseAbility;

    public Action<int, bool> OnPop 
    { 
        get {  return score.OnPop; } 
        set { score.OnPop = value; }
    }

    public Action<int> OnStreak
    {
        get { return score.OnStreak; }
        set { score.OnStreak = value; }
    }

    public Action OnStreakTimeEnd
    {
        get { return score.OnStreakTimeEnd; }
        set { score.OnStreakTimeEnd = value; }
    }

    public Action OnAbilityShot
    {
        get { return ability.OnShot; }
        set { ability.OnShot = value; }
    }

    public int HighStreak => score.HighStreak;
    public int CurrentStreak => score.CurrentStreak;

    private void Awake()
    {
        canUseAbility = false;

        ui = GetComponent<NeedleUI>();
        score = GetComponent<NeedleScore>();
        movement = GetComponent<NeedleMovement>();
        ability = GetComponent<NeedleAbility>();

        score.OnStreakEnd += ui.ClearStreak;

        score.OnStreak += (streak) =>
        {
            if (streak >= 3)
            {
                ui.StartStreakTimer(score.StreakDuration);
            }

            ui.UpdateStreak(streak);
        };
        
        ability.OnShot += () =>
        {
            ui.ShowAbility(ability.AbilityCount);
            ui.HideAbilityDelay(2f);
        };

        ability.OnStopAim += () => 
        {
            ui.HideAbility();
        };

        ability.OnEmpty += ui.ShowAbilityEmpty;

        NeedleShotable.OnShotablePop += (score, isBubbleGold) =>
        {
            this.score.AddScore(score, isBubbleGold);
        };

        ability.OnAbilityAdded += (abilityCount) =>
        {
            ui.ShowAbility(abilityCount, doCancelInvokes: true);
        };

        ability.OnAddAbilityEnded += () =>
        {
            ui.HideAbility();
            canUseAbility = true;
        };
    }

    private void Start()
    {
        ResetNeedle();
    }

    private void Update()
    {
        if (!canUseAbility) return;

        if (Input.GetMouseButtonDown(1))
        {
            ability.StopAim();
        }

        if (Input.GetMouseButtonDown(0))
        {
            ability.ActivateAim();

            if (ability.IsEmpty == false)
            {
                ui.ShowAbility(ability.AbilityCount, true, true);
            }
        }
        else if (Input.GetMouseButtonUp(0)) 
        {
            ability.ShotNeedle();
        }

        if (ability.IsAiming == true)
        {
            Vector2 position = (Vector2) transform.position + (Vector2.up * 0.25f);
            ability.MoveAim(position, 7.5f);
        }
    }

    public void SetPosition(Vector2 position)
    {
        movement.SetPosition(position);
    }

    public void ResetNeedle()
    { 
        score.ResetStats(); 
        ability.ResetAbility();

        ui.HideAbility();
        ui.ClearStreak();
        ui.ShowAbility(ability.AbilityCount);
        ui.HideAbilityDelay(3f);
    }

    public void ClearStreak()
    {
        ui.ClearStreak();
    }

    public void HealCallback(bool streakHeal, bool goldHeal)
    {
        if(streakHeal == true && goldHeal == false)
        {
            ui.SetHealActive(true, 1);
        }
        else if (streakHeal == false && goldHeal == true)
        {
            ui.SetHealActive(true, 2);
        }
        else if (streakHeal == true && goldHeal == true)
        {
            ui.SetHealActive(true, 3);
        }
        else if (streakHeal == false && goldHeal == false)
        {
            ui.SetHealActive(false);
        }
    }

    public void SetAbilityActive(bool newIsActive)
    {
        canUseAbility = newIsActive;
    }

    public void ActivateAbilityDelay(float delay)
    {
        Invoke(nameof(ActivateAbility), delay);
    }

    private void ActivateAbility()
    {
        canUseAbility = true;
    }

    public void AddAbility(int count)
    {
        canUseAbility = false;

        ui.ShowAbility(ability.AbilityCount, doCancelInvokes: true);
        ability.AddAbility(count);
        //ui.ShowAbilityCount(ability.AbilityCount);
    }
}
