using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindMovement : MonoBehaviour
{
    private new Rigidbody2D rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    public void Blow(float force)
    {
        rigidbody.AddForce(Vector2.down * force, ForceMode2D.Impulse);
    }
}
