using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleSimple : Bubble
{
    private const float MAX_ABSORB_TIME = 4f;
    private const float MAX_GRAVITY = 2.5f;

    [SerializeField] private float attractionForce;
    [SerializeField] private float velocityLimit;
    [SerializeField] private Color[] colors;

    private int absorbCount;
    private bool doLimitSpeed;
    private float timerAbsorb;
    private float initialGravity;
    private Vector3 initialScale;
    private SpriteRenderer sprite;
    private List<Bubble> ignore;

    public new static Action<Vector2, float> OnBubblePop;

    protected override void OnAwake()
    {
        initialGravity = rigidbody.gravityScale;
        initialScale = transform.localScale;
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    protected override void ResetStats()
    {
        rigidbody.gravityScale = initialGravity;
        doLimitSpeed = false;
        absorbCount = 0;
        transform.localScale = initialScale;
        base.ResetStats();
        sprite.color = colors[0];
        ignore = new List<Bubble>();
        timerAbsorb = 0f;
    }

    private void AbsorbBubble(int score)
    {
        ++absorbCount;
        this.score += score;

        if (this.score >= 20)
        {
            sprite.color = colors[1];
        }
        if (this.score >= 40)
        {
            sprite.color = colors[2];
        }

        transform.localScale = initialScale * (1f + 0.15f * absorbCount);

        rigidbody.gravityScale += initialGravity * MathF.Pow(1.1f, absorbCount);
        if (rigidbody.gravityScale >= MAX_GRAVITY)
        {
            rigidbody.gravityScale = MAX_GRAVITY;
        }
    }

    protected override void VirtualFixedUpdate()
    {
        if (doLimitSpeed == false) return;

        if(rigidbody.velocity.magnitude >= velocityLimit)
        {
            rigidbody.velocity = rigidbody.velocity.normalized * velocityLimit;
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.transform.CompareTag("Bubble"))
        {
            Bubble newBubble = collider.transform.GetComponent<Bubble>();

            if (ignore.Contains(newBubble))
            {
                return;
            }

            if (newBubble is BubbleSimpleSmall)
            {
                doLimitSpeed = true;

                Vector2 difference = newBubble.transform.position - transform.position;
                newBubble.MoveTowards(-difference.normalized, attractionForce);

                timerAbsorb += Time.deltaTime;

                if (timerAbsorb >= MAX_ABSORB_TIME)
                {
                    ignore.Add(newBubble);

                    timerAbsorb = 0f;
                }
            }
            else if (newBubble is BubbleRussian)
            {
                bool isSmall = (newBubble as BubbleRussian).IsSmall;
                if (isSmall)
                {
                    doLimitSpeed = true;

                    Vector2 difference = newBubble.transform.position - transform.position;
                    newBubble.MoveTowards(-difference.normalized, attractionForce);
                    
                    timerAbsorb += Time.deltaTime;

                    if(timerAbsorb >= MAX_ABSORB_TIME)
                    {
                        ignore.Add(newBubble);

                        timerAbsorb = 0f;
                    }
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Bubble"))
        {
            Bubble newBubble = collision.transform.GetComponent<Bubble>();

            if (newBubble is BubbleSimpleSmall)
            {
                doLimitSpeed = false;
                newBubble.StartMoving();
            }
            else if (newBubble is BubbleRussian)
            {
                bool isSmall = (newBubble as BubbleRussian).IsSmall;
                if (isSmall)
                {
                    doLimitSpeed = false;
                    newBubble.StartMoving();
                }
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Bubble"))
        {
            Bubble newBubble = collision.transform.GetComponent<Bubble>();

            if (newBubble is BubbleSimpleSmall)
            {
                int score = newBubble.PopTotally(true, true);
                if (score != default)
                {
                    AbsorbBubble(score);
                }
            }
            else if (newBubble is BubbleRussian)
            {
                bool isSmall = (newBubble as BubbleRussian).IsSmall;
                if (isSmall)
                {
                    int score = newBubble.Pop();
                    if (score != default)
                    {
                        AbsorbBubble(score);
                    }
                }
            }
        }
    }

    public override int Pop(bool useParticles = true)
    {
        OnBubblePop?.Invoke(transform.position, transform.localScale.x);

        return base.Pop(false);
    }
}
