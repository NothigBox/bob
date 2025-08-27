using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NeedleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI streak;
    [SerializeField] private TextMeshProUGUI heal;
    [SerializeField] private TextMeshProUGUI ability;
    [SerializeField] private GameObject cancel;
    [SerializeField] private Animation empty;
    [SerializeField] private Image streakTime;
    [SerializeField] private float abilityCountDuration;

    private Color initialAbility;

    private float? streakTimer;
    private float streakDuration;
    private bool canUpdateHeal;

    private void Awake()
    {
        canUpdateHeal = true;
        streakTimer = null;
        initialAbility = ability.color;
    }

    private void FixedUpdate()
    {
        if (streakTimer == null) return;

        if (streakTimer >= streakDuration)
        {
            StopStreakTimer();
        }
        else
        {
            streakTime.fillAmount = (float)(0.75f - ((streakTimer/streakDuration) * 0.75f));
        }

        streakTimer += Time.fixedDeltaTime;
    }

    public void StartStreakTimer(float streakDuration)
    {
        this.streakDuration = streakDuration;
        streakTimer = 0f;
        streakTime.gameObject.SetActive(true);
    }

    public void StopStreakTimer()
    {
        streakTimer = null;
        streakTime.gameObject.SetActive(false);
    }

    public void UpdateStreak(int streak)
    {
        this.streak.text = "x" + streak.ToString();
    }

    public void ClearStreak()
    {
        streak.text = "";
        SetHealActive(false);
        StopStreakTimer();
    }

    public void SetHealActive(bool newIsActive, int heal = 0)
    {
        if (newIsActive == false)
        {
            canUpdateHeal = true;
        }

        if (!canUpdateHeal) return;
        if(newIsActive == true) canUpdateHeal = false;

        this.heal.gameObject.SetActive(newIsActive);
        this.heal.text = "+" + heal.ToString();
    }

    public void ShowAbility(int abilityCount, bool doShowCancel = false, bool doCancelInvokes = false)
    {
        if(doCancelInvokes == true)
        {
            CancelInvoke();
        }

        ability.color = initialAbility;
        ability.gameObject.SetActive(true);
        ability.text = "x" + abilityCount.ToString();
        
        if (doShowCancel == true)
        {
            cancel.SetActive(true);
        }
    }

    public void HideAbilityDelay(float delay)
    {
        cancel.SetActive(false);
        Invoke(nameof(HideAbility), delay);
    }

    public void HideAbility()
    {
        cancel.SetActive(false);
        ability.gameObject.SetActive(false);
    }

    public void ShowAbilityEmpty()
    {
        StopAllCoroutines();
        StartCoroutine(ShowAbilityEmptyCoroutine());
    }

    IEnumerator ShowAbilityEmptyCoroutine()
    {
        ShowAbility(0, doCancelInvokes: true);

        empty.Play();

        yield return new WaitForSeconds(abilityCountDuration);

        HideAbility();
    }
}
