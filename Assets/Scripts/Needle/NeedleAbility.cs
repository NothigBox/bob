using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class NeedleAbility : MonoBehaviour
{
    const int INITIAL_ABILITY_COUNT = 3;

    [SerializeField] private NeedleShotable shotable;
    [SerializeField] private float force;
    [SerializeField] private new LineRenderer renderer;

    private ObjectsPool<NeedleShotable> pool;
    private bool? hasStopedAiming;
    private int abilityCount;

    public bool IsAiming => hasStopedAiming == null? false : !hasStopedAiming.Value;
    public int AbilityCount => abilityCount;
    public bool IsEmpty => abilityCount <= 0;

    public Action OnEmpty;
    public Action OnShot;
    public Action OnStopAim;
    public Action<int> OnAbilityAdded;
    public Action OnAddAbilityEnded;

    private void Awake()
    {
        pool = new ObjectsPool<NeedleShotable>(shotable);

        ResetAbility();
    }

    public void ShotNeedle()
    {
        if(abilityCount <= 0) return;

        if (hasStopedAiming == true) 
        {
            hasStopedAiming = null;
            return; 
        }

        for (int i = -1; i <= 1; i++)
        {
            NeedleShotable newNeedle = pool.GetObject(transform.position);
            newNeedle.Shot(new Vector2(0.2f * i, 1f) , force);
        }

        abilityCount--;

        OnShot?.Invoke();

        StopAim(false);
    }

    public  void ActivateAim()
    {
        if (abilityCount <= 0)
        {
            OnEmpty?.Invoke();
            return;
        }

        hasStopedAiming = false;
    }

    public void MoveAim(Vector2 startPosition, float length) 
    {
        renderer.SetPosition(0, startPosition);

        renderer.SetPosition(1, new Vector2(startPosition.x, length));

        renderer.gameObject.SetActive(true);
    }

    public void StopAim(bool invokeStopAim = true)
    {
        hasStopedAiming = true;
        renderer.gameObject.SetActive(false);

        if (invokeStopAim == true)
        {
            OnStopAim?.Invoke();
        }
    }

    public void ResetAbility()
    {
        abilityCount = INITIAL_ABILITY_COUNT;
        hasStopedAiming = null;
    }

    public void AddAbility(int count)
    {
        StopAllCoroutines();
        StartCoroutine(AddAbilityCoroutine(count));
    }

    IEnumerator AddAbilityCoroutine(int addCount)
    {
        float timeBetweenAbilityCounts = 0.75f;
        int newAbilityCount = abilityCount + addCount;

        for (int i = abilityCount; i <= newAbilityCount; i++) 
        {
            yield return new WaitForSeconds(timeBetweenAbilityCounts);
            OnAbilityAdded?.Invoke(i);
        }

        abilityCount = newAbilityCount;

        yield return new WaitForSeconds(timeBetweenAbilityCounts);

        OnAddAbilityEnded?.Invoke();
    }
}