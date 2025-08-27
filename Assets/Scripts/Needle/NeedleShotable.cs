using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class NeedleShotable : PoolObject
{
    private new Rigidbody2D rigidbody;

    public static Action<int, bool> OnShotablePop;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    public void Shot(Vector2 direction, float force)
    {
        rigidbody.AddForce(direction * force, ForceMode2D.Impulse);
        
        if (direction.x < 0)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 10f);
        }
        else if (direction.x > 0)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, -10f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bubble"))
        {
            Bubble bubble = collision.GetComponent<Bubble>();
            int score = bubble.PopTotally();

            OnShotablePop?.Invoke(score, bubble is BubbleGold);
        }
        else if (collision.CompareTag("Roof"))
        {
            gameObject.SetActive(false);
        }
    }
}
