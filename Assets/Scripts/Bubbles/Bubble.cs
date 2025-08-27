using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using RANDOM = UnityEngine.Random;

public abstract class Bubble : MonoBehaviour
{
    const float MAX_SPEED_MOVING_TOWARDS = 10f;

    [SerializeField] protected EBubbleSize size;

    [Space]
    [SerializeField] protected int score;
    [SerializeField] protected ParticleSystem fall;

    protected float movementForce;
    protected float movementDuration;

    protected new Rigidbody2D rigidbody;

    private EBubbleSize initialSize;
    private Vector2 movementDirection;
    private float timerMovementDuration;
    private int initialScore;
    private bool justStarted;
    private bool isMovingTowards;
    private bool canUseWind;

    protected bool isMoving;

    public Action<Bubble> OnDisabled;
    public static Action<Vector2, EBubbleSize> OnBubblePop;

    public int Score => score;

    private void Awake()
    {
        initialScore = score;
        initialSize = size;
        
        rigidbody = GetComponent<Rigidbody2D>();

        OnAwake();
    }

    private void OnEnable()
    {
        ResetStats();
    }

    void FixedUpdate()
    {
        if (!isMoving) return;

        //  When start moving, do only the half of the movement
        if (justStarted)
        {
            if (timerMovementDuration >= movementDuration / 2f)
            {
                justStarted = false;
                movementDirection *= -1;
                timerMovementDuration = 0f;
            }
        }
        else
        {
            if (timerMovementDuration >= movementDuration)
            {
                movementDirection *=  -1;
                timerMovementDuration = 0f;
            }
        }

        rigidbody.AddForce(movementDirection * (movementForce * (isMovingTowards? 0f : 1f)), ForceMode2D.Force);

        timerMovementDuration += Time.fixedDeltaTime;

        if (isMovingTowards)
        {
            if(rigidbody.velocity.magnitude >= MAX_SPEED_MOVING_TOWARDS)
            {
                rigidbody.velocity = rigidbody.velocity.normalized * MAX_SPEED_MOVING_TOWARDS;
            }
        }

        VirtualFixedUpdate();
    }

    protected virtual void VirtualFixedUpdate()
    { 

    }

    public void Blow(Vector2 force)
    {
        rigidbody.velocity = Vector2.zero;
        rigidbody.AddForce(force, ForceMode2D.Impulse);
        fall.Stop();

        Invoke(nameof(StartMoving), 1f);
    }

    public void StartMoving(float movementForce, float movementDuration, float gravityScale)
    {
        this.movementForce = movementForce;
        this.movementDuration = movementDuration;
        rigidbody.gravityScale = gravityScale;

        isMoving = true;
        isMovingTowards = false;
        fall.Play();
    }

    public virtual void StartMoving()
    {
        float randomForce = RANDOM.Range(2f, 5f);
        float randomDuration = RANDOM.Range(1f, 3f);
        float randomGravityScale = RANDOM.Range(0.045f, 0.085f);

        StartMoving(randomForce, randomDuration, randomGravityScale);
    }

    public void MoveTowards(Vector2 direction, float force)
    {
        isMovingTowards = true;
        rigidbody.AddForce(direction * force, ForceMode2D.Force);
        fall.Stop();
    }

    public virtual int Pop(bool useParticles = true)
    {
        if (isMoving == false) return default;
        isMoving = false;

        if(useParticles)
        { 
            OnBubblePop?.Invoke(transform.position, size); 
        }

        StopAllCoroutines();

        gameObject.SetActive(false);

        return score;
    }

    public virtual int PopTotally(bool useParticles = true, bool doForce = false)
    {
        if (isMoving == false && doForce == false) return default;
        isMoving = false;

        //  Could cause a bug related to the simple bubble particles in the near future.
        if (useParticles)
        {
            OnBubblePop?.Invoke(transform.position, size);
        }

        StopAllCoroutines();

        gameObject.SetActive(false);

        return score;
    }

    protected virtual void ResetStats()
    {
        isMoving = false;
        isMovingTowards = false;
        canUseWind = true;
        justStarted = true;
        score = initialScore;
        size = initialSize;
        timerMovementDuration = 0f;
        movementDirection = Vector2.right;
    }

    protected virtual void OnAwake()
    {

    }

    private void OnDisable()
    {
        OnDisabled?.Invoke(this);
    }

    public void ApplyWind(float windForce)
    {
        if (canUseWind == false) return;
        canUseWind = false;
        
        rigidbody.AddForce(Vector2.down * windForce, ForceMode2D.Impulse);

        Invoke(nameof(ActivateWind), 5f);
    }

    public void ActivateWind()
    {
        canUseWind = true;
    }
}
