using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class NeedleMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float swingForce;

    private float previousPositionX;

    public void SetPosition(Vector2 position)
    {
        if (position.x != previousPositionX)
        {
            int direction = position.x > previousPositionX? -1 : 1;

            rb.angularVelocity = 0f;
            rb.AddTorque(swingForce * direction);
        }

        transform.position = position;
        previousPositionX = position.x;
    }
}
