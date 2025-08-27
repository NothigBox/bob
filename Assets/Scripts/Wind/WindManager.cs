using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RANDOM = UnityEngine.Random;

[RequireComponent(typeof(WindMovement))]

public class WindManager : PoolObject
{
    [SerializeField] private SpriteRenderer[] sprites;

    private float force;
    private WindMovement movement;

    private void OnEnable()
    {
        movement = GetComponent<WindMovement>();
        sprites = GetComponentsInChildren<SpriteRenderer>();

        for(int i = 0; i < sprites.Length; i++)
        {
            int randomBool = RANDOM.Range(0, 2);

            sprites[i].flipX = randomBool == 1;
        }
    }

    public void Blow(float force)
    {
        this.force = force;
        movement.Blow(force);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bubble"))
        {
            Bubble newBubble = collision.GetComponent<Bubble>();
            newBubble.ApplyWind(force);
        }
        else if (collision.CompareTag("FinishWind"))
        {
            gameObject.SetActive(false);
        }
    }
}
