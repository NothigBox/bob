using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleMultiple : Bubble
{
    [SerializeField] private Bubble bubble;
    [SerializeField] private Vector2 bubbleBlowForce;

    public static Action<Vector2, Vector2> OnMultiplePop;

    public override int Pop(bool useParticles = true)
    {
        Vector2 position = transform.position + Vector3.up;
        OnMultiplePop?.Invoke(position, bubbleBlowForce);

        return base.Pop();
    }
}
