using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleRussian : Bubble
{
    [SerializeField] GameObject[] bubbles;
    [SerializeField] private float invulnerabilityDuration;

    private int currentLifes;
    private BoxCollider2D[] boxColliders;
    private Vector2[] initialBoxSizes;

    private bool isSmall;
    private bool isInvulnerable;

    public bool IsSmall => isSmall;

    private int MAX_LIFES => bubbles.Length;

    protected override void OnAwake()
    {
        boxColliders = GetComponents<BoxCollider2D>();
        initialBoxSizes = new Vector2[boxColliders.Length];

        for(int i = 0; i < boxColliders.Length; i++)
        {
            initialBoxSizes[i] = boxColliders[i].size;
            initialBoxSizes[i] = boxColliders[i].size;
        }
    }

    protected override void ResetStats()
    {
        isSmall = false;
        isInvulnerable = false;
        currentLifes = MAX_LIFES;

        SetSize(EBubbleSize.Big);        

        for (int i = 0; i < bubbles.Length; i++)
        {
            bubbles[i].gameObject.SetActive(true);
        }

        base.ResetStats();
    }

    public override int Pop(bool useParticles = true)
    {
        if (isInvulnerable) return default;

        StartCoroutine(InvulnerabilityCoroutine());

        --currentLifes;

        //if(currentLifes > 0)
        {
            //  
            for (int i = MAX_LIFES; i > currentLifes; i--)
            {
                bubbles[i - 1].SetActive(false);
            }
        }

        Blow(Vector2.up * 5f);

        switch (currentLifes)
        {
            case 2:
                OnBubblePop?.Invoke(transform.position, size);

                //  Set mid stats
                SetSize(EBubbleSize.Mid);
                break;

            case 1:
                //  Set mid Pop stats
                score = 10;
                OnBubblePop?.Invoke(transform.position, size);

                //  Set small stats
                SetSize(EBubbleSize.Lil);
                break;
        }

        if (currentLifes <= 0)
        {
            //  Set small Pop stats
            score = 5;
            //size = EBubbleSize.Lil;

            base.Pop();
        }

        return score;
    }

    private void SetSize(EBubbleSize size)
    {
        switch (size)
        {
            case EBubbleSize.Big:
                boxColliders[0].size = initialBoxSizes[0];
                boxColliders[1].size = initialBoxSizes[1];

                fall.transform.localPosition = Vector3.down * 0.65f;
                fall.transform.localScale = new Vector3(1.25f, 1f, 1f);
                break;

            case EBubbleSize.Mid:
                boxColliders[0].size = new Vector2(0.6f, 1f);
                boxColliders[1].size = new Vector2(0.9f, 0.75f);

                fall.transform.localPosition = Vector3.down * 0.357f;
                fall.transform.localScale = Vector3.one;
                break;

            case EBubbleSize.Lil:
                isSmall = true;
                boxColliders[0].size = new Vector2(0.3f, 0.5f);
                boxColliders[1].size = new Vector2(0.45f, 0.35f);

                fall.transform.localPosition = Vector3.down * 0.1f;
                fall.transform.localScale = new Vector3(0.66f, 1f, 1f);
                break;
        }

        this.size = size;
    }

    IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;

        yield return new WaitForSeconds(invulnerabilityDuration);

        isInvulnerable = false;
    }
}
