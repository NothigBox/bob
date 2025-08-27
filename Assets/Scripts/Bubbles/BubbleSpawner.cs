using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class BubbleSpawner : MonoBehaviour
{
    [SerializeField] private float windForce;
    [SerializeField] private float timeBetweenWind;
    [SerializeField] private BubbleFactory factory;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] bubbleSpawnPoints;

    [SerializeField] private float[] probabilities;

    public Action<int> OnAllBubblesPopped;

    private Transform RandomSpawnPoint 
    {
        get 
        {
            int randomSpawnPointIndex = UnityEngine.Random.Range(0, bubbleSpawnPoints.Length);

            Transform spawnPoint = bubbleSpawnPoints[randomSpawnPointIndex];

            return spawnPoint;
        } 
    }

    private void Awake()
    {
        BubbleMultiple.OnMultiplePop += SpawnMultipleBubble;
        Bubble.OnBubblePop += SpawnBubblePopParticle;
        BubbleSimple.OnBubblePop += SpawnBubblePopParticle;
    }

    public void SpawnBubblesAtRandomPoints(int count)
    {
        for (int i = 0; i < count; i++)
        {
            float randomBubble = UnityEngine.Random.Range(0f, 100f);

            Bubble newBubble = null;

            if (randomBubble <= AcumulatedProbability(0))
            {
                newBubble = factory.GetBubbleSimple(RandomSpawnPoint.position);
            }
            else if (randomBubble <= AcumulatedProbability(1))
            {
                newBubble = factory.GetBubbleMultiple(RandomSpawnPoint.position);
            }
            else if (randomBubble <= AcumulatedProbability(2))
            {
                newBubble = factory.GetBubbleRussian(RandomSpawnPoint.position);
            }        

            if(newBubble != null)
            {
                newBubble.StartMoving();
            }
        }
    }

    public void SpawnGoldBubblesAtRandomPoints(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Bubble newBubble = factory.GetBubbleGold(RandomSpawnPoint.position);

            newBubble.StartMoving();
        }
    }

    private void SpawnMultipleBubble(Vector2 position, Vector2 blowForce)
    {
        for (int i = -1; i <= 1; i++)
        {
            Bubble newBubble = factory.GetBubbleSmall(position);
            newBubble.Blow(new Vector2(i * blowForce.x, blowForce.y));
        }
    }

    private void SpawnBubblePopParticle(Vector2 position, EBubbleSize size)
    {
        factory.GetBubblePopParticle(position, size);
    }

    private void SpawnBubblePopParticle(Vector2 position, float scale)
    {
        factory.GetBubblePopParticle(position, scale);
    }

    private float AcumulatedProbability(int index)
    {
        float result = 0f;

        for (int i = 0; i <= index; i++)
        {
            result += probabilities[i];
        }

        return result;
    }

    public void SetProbabilities(float[] probabilities)
    {
        this.probabilities = probabilities;
    }

    public void KillAllBubbles(bool sendScore)
    {
        Bubble[] activeBubbles = factory.GetAllActiveBubbles().ToArray();

        int popScore = 0;

        for (int i = 0; i < activeBubbles.Length; i++) 
        {
            popScore += activeBubbles[i].PopTotally(true, true);
        }

        if (sendScore)
        {
            OnAllBubblesPopped?.Invoke(popScore);
        }
    }

    public void SetWind(float windForce)
    {
        var a = factory.GetAllActiveBubbles(true);

        for (int i = 0; i < a.Count; i++) 
        {
            a[i].ApplyWind(windForce);
        }
    }

    float timer = 0f;

    private void FixedUpdate()
    {
        return;

        if (timer >= timeBetweenWind)
        {
            timer = 0f;

            SetWind(windForce);
        }
        timer += Time.fixedDeltaTime;
    }
}
