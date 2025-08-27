using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubblePopParticle : PoolObject
{
    private ParticleSystem particle;

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        particle.Play();
        Invoke(nameof(TurnOff), particle.main.duration);
    }

    public void SetSize(float scale)
    {
        transform.localScale = Vector3.one * scale;
    }

    public void SetSize(EBubbleSize size)
    {
        Vector3 scale = default;

        switch (size)
        {
            case EBubbleSize.Lil:
                scale = Vector3.one * 0.5f;
                break;

            case EBubbleSize.Mid:
                scale = Vector3.one * 1f;
                break;

            case EBubbleSize.Big:
                scale = Vector3.one * 1.5f;
                break;
        }

        transform.localScale = scale;
    }

    private void TurnOff()
    {
        gameObject.SetActive(false);
    }
}

public enum EBubbleSize { Lil, Mid, Big, Other }