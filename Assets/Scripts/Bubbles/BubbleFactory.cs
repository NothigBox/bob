using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BubbleFactory : MonoBehaviour
{

    [SerializeField] private BubbleSimple simple;
    [SerializeField] private BubbleMultiple multiple;
    [SerializeField] private BubbleRussian russian;
    [SerializeField] private BubbleSimpleSmall small;
    [SerializeField] private BubbleGold gold;
    [SerializeField] private BubblePopParticle bubblePopParticle;

    private BubblesPool<BubbleSimple> bubbleSimple;
    private BubblesPool<BubbleMultiple> bubbleMultiple;
    private BubblesPool<BubbleRussian> bubbleRussian;
    private BubblesPool<BubbleSimpleSmall> bubbleSmall;
    private BubblesPool<BubbleGold> bubbleGold;
    private ObjectsPool<BubblePopParticle> bubblePopParticles;    

    private void Awake()
    {
        bubbleSimple = new BubblesPool<BubbleSimple>(simple);
        bubbleMultiple = new BubblesPool<BubbleMultiple>(multiple);
        bubbleRussian = new BubblesPool<BubbleRussian>(russian);
        bubbleSmall = new BubblesPool<BubbleSimpleSmall>(small);
        bubbleGold = new BubblesPool<BubbleGold>(gold);
        bubblePopParticles = new ObjectsPool<BubblePopParticle>(bubblePopParticle);
    }

    public Bubble GetBubbleSimple(Vector3 position)
    {
        return bubbleSimple.GetBubble(position);
    }

    public Bubble GetBubbleMultiple(Vector3 position)
    {
        return bubbleMultiple.GetBubble(position);
    }

    public Bubble GetBubbleRussian(Vector3 position)
    {
        return bubbleRussian.GetBubble(position);
    }

    public Bubble GetBubbleSmall(Vector3 position)
    {
        return bubbleSmall.GetBubble(position);
    }

    public Bubble GetBubbleGold(Vector3 position)
    {
        return bubbleGold.GetBubble(position);
    }

    public BubblePopParticle GetBubblePopParticle(Vector3 position, EBubbleSize size)
    {
        BubblePopParticle newBubble = bubblePopParticles.GetObject(position);
        newBubble.SetSize(size);

        return newBubble;
    }

    public BubblePopParticle GetBubblePopParticle(Vector3 position, float scale)
    {
        BubblePopParticle newBubble = bubblePopParticles.GetObject(position);
        newBubble.SetSize(scale);

        return newBubble;
    }

    public List<Bubble> GetAllActiveBubbles(bool includeGold = false)
    {
        List<Bubble> result = new List<Bubble>();

        result.AddRange(bubbleSimple.GetActiveBubbles());
        result.AddRange(bubbleMultiple.GetActiveBubbles());
        result.AddRange(bubbleRussian.GetActiveBubbles());
        result.AddRange(bubbleSmall.GetActiveBubbles());

        if (includeGold) 
        {
            result.AddRange(bubbleGold.GetActiveBubbles());
        }

        return result;
    }
}
