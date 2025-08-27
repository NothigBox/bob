using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RANDOM = UnityEngine.Random;

public class BubbleGold : BubbleSimpleSmall
{
    public override void StartMoving()
    {
        float randomForce = RANDOM.Range(2f, 3f);
        float randomDuration = RANDOM.Range(0.5f, 1.5f);
        float randomGravityScale = RANDOM.Range(0.035f, 0.075f);

        StartMoving(randomForce, randomDuration, randomGravityScale);

        if (gameObject.activeSelf == true)
        {
            StartCoroutine(WanderCoroutine(randomDuration));
        }
    }

    private void Wander()
    {
        float randomForce = RANDOM.Range(1f, 6f);
        float randomDuration = RANDOM.Range(0.3f, 1.7f);

        movementForce = randomForce;
        movementDuration = randomDuration;

        if (gameObject.activeSelf == true)
        {
            StartCoroutine(WanderCoroutine(randomDuration));
        }
    }

    IEnumerator WanderCoroutine(float movementDuration)
    {
        yield return new WaitForSeconds(movementDuration);

        Wander();

        yield return null;
    }
}
