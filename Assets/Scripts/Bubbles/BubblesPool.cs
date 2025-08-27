using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubblesPool<T> : Object where T : Bubble
{
    private T bubble;

    private List<T> allBubbles;
    private List<T> availableBubbles;
    private List<T> unavailableBubbles;

    public List<T> AllBubbles 
    { 
        get 
        {
            if(allBubbles == null)
            {
                allBubbles = new List<T>();
            }

            return allBubbles;
        }
        set 
        {
            allBubbles = value;
        }
    }
    public List<T> AvailableBubbles
    {
        get
        {
            if (availableBubbles == null)
            {
                availableBubbles = new List<T>();
            }

            return availableBubbles;
        }
        set
        {
            availableBubbles = value;
        }
    }
    public List<T> UnavailableBubbles
    {
        get
        {
            if (unavailableBubbles == null)
            {
                unavailableBubbles = new List<T>();
            }

            return unavailableBubbles;
        }
        set
        {
            unavailableBubbles = value;
        }
    }

    public BubblesPool(T prefab)
    {
        bubble = prefab;
    }

    private T GetNewBubble(Vector3 position)
    {
        T bubble = Instantiate(this.bubble, position, Quaternion.identity);

        bubble.OnDisabled += ReceiptBubble;
        
        AllBubbles.Add(bubble);
        UnavailableBubbles.Add(bubble);

        return bubble;
    }

    public T GetBubble(Vector3 position) 
    {
        T bubble = default;

        if (AvailableBubbles.Count <= 0)
        {
            bubble = GetNewBubble(position);
        }
        else
        {
            bubble = AvailableBubbles[0];
             
            AvailableBubbles.RemoveAt(0);
            UnavailableBubbles.Add(bubble);

            bubble.transform.position = position;
        }

        bubble.gameObject.SetActive(true);

        return bubble;
    }

    private void ReceiptBubble(Bubble bubble)
    {
        UnavailableBubbles.Remove(bubble as T);
        AvailableBubbles.Add(bubble as T);
    }

    public List<Bubble> GetActiveBubbles()
    {
        List<Bubble> result = new List<Bubble>();

        for (int i = 0; i < UnavailableBubbles.Count; i++)
        {
            result.Add(UnavailableBubbles[i]);
        }

        return result;
    }
}
