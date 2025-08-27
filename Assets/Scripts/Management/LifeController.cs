using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeController : MonoBehaviour
{
    [SerializeField] private int maxLife;
    
    private int currentLife;
    private int hurtCount;

    public Action<int> OnLifeUpdate;
    public Action OnDamaged;

    public int HurtCount => hurtCount;

    private void Start()
    {
        SetMaxLife();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bubble"))
        {
            Bubble newBubble = collision.GetComponent<Bubble>();

            if (newBubble is BubbleGold)
            {
                newBubble.Pop();
            }
            else
            {
                RestLife(newBubble.Pop());
            }
        }
    }

    public void RestLife(int score)
    {
        ++hurtCount;
        if(hurtCount > 4)
        {
            hurtCount = 4;
        }

        currentLife -= score;

        if (currentLife < 0)
        {
            currentLife = 0;
        }

        if (currentLife >= 0)
        {
            OnLifeUpdate?.Invoke(currentLife);
            OnDamaged?.Invoke();
        }
    }

    public bool AddLife(int streak)
    {
        bool didAddLife = false;

        if(currentLife < maxLife)
        {
            didAddLife = true;
        }

        currentLife += streak;

        if (currentLife > maxLife) 
        {
            currentLife = maxLife;
        }

        OnLifeUpdate?.Invoke(currentLife);

        return didAddLife;
    }

    public void SetMaxLife()
    {
        currentLife = maxLife;

        OnLifeUpdate?.Invoke(currentLife);
    }

    public void ClearHurtCount()
    {
        hurtCount = 0;
    }
}
