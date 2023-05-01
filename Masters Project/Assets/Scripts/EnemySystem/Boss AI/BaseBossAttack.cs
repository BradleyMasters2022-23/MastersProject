using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

public abstract class BaseBossAttack : TimeAffectedEntity, TimeObserver
{
    public enum AttackState
    {
        Ready,
        Preparing,
        Attacking,
        Recovering,
        Stunned,
        Cooldown
    }

    [Header("Boss Attack")]

    [SerializeField, ReadOnly] protected AttackState state;

    [SerializeField] protected GameObject projectilePrefab;

    [SerializeField] protected Transform shootPoint;

    [SerializeField] protected int numberOfAttacks = 1;

    protected List<RangeAttack> spawnedProjectiles;

    [SerializeField] protected float phaseChangeDuration;

    [SerializeField, Range(0, 1)] protected float lossTargetVisionCone;

    [SerializeField] protected float lossTargetStunDuration;

    [SerializeField] protected bool lossTargetInturruptAttack;

    [SerializeField, ReadOnly] protected float speedModifier = 1;

    private Vector3 restingPosition;

    protected Target target;

    protected Rigidbody targetRB;

    [Header("Attack State")]

    [SerializeField] protected float attackRotationSpeed;

    [SerializeField] protected float attackDuration;


    [Header("Recovery State")]

    [SerializeField] protected float recoveryDuration;

    [SerializeField] protected float recoveryRotationSpeed;

    [SerializeField] protected Vector2 cooldownRange;

    protected int currentPhase;

    protected Coroutine currentRoutine;

    protected ScaledTimer tracker;

    protected ScaledTimer stunnedTracker;

    protected WaitForFixedUpdate frame;

    protected Vector3 originalPosVector;

    #region Setup

    protected virtual void Awake()
    {
        // If not set, use normal transform
        if (shootPoint == null)
            shootPoint = transform;

        spawnedProjectiles = new List<RangeAttack>();

        target = FindObjectOfType<PlayerTarget>().GetComponent<PlayerTarget>();
        targetRB = target.gameObject.GetComponent<Rigidbody>();

        frame = new WaitForFixedUpdate();

        tracker = new ScaledTimer(0, Affected);
        stunnedTracker = new ScaledTimer(lossTargetStunDuration, Affected);

        SetRestPosition();
    }

    protected virtual void UpdateTimers()
    {
        if(Affected)
        {
            //Debug.Log("Setting modifiers to " + Timescale);
            tracker?.SetModifier(Timescale);
            stunnedTracker?.SetModifier(Timescale);
        }
    }

    /// <summary>
    /// Set the rest position to current position
    /// </summary>
    public void SetRestPosition()
    {
        restingPosition = transform.forward * 50;
        Debug.DrawLine(transform.position, restingPosition, Color.red, 10f);
    }

    /// <summary>
    /// Whether or not this attack can be performed
    /// </summary>
    /// <returns></returns>
    public virtual bool CanAttack()
    {
        return state == AttackState.Ready && isActiveAndEnabled && currentRoutine == null;
    }

    /// <summary>
    /// Increment the speed modifier for the attack
    /// </summary>
    /// <param name="increment">Amount to increment by</param>
    public void AddSpeedModifier(float increment)
    {
        speedModifier += increment;
    }

    public bool AttackDone()
    {
        return state == AttackState.Ready;
    }

    #endregion

    /// <summary>
    /// Perform this attack
    /// </summary>
    public void Attack()
    {
        state = AttackState.Ready;

        currentRoutine = StartCoroutine(AttackRoutine());
    }

    private void Update()
    {
        UpdateTimers();

        if (state == AttackState.Stunned)
        {
            if (stunnedTracker.TimerDone())
                state = AttackState.Attacking;
        }
    }

    public abstract IEnumerator AttackRoutine();

    #region Virtual Attack Functions

    /// <summary>
    /// Cancel the current attack, if possible
    /// </summary>
    public virtual void CancelAttack()
    {
        StartCoroutine(Cancel());
    }

    /// <summary>
    /// Cancel the current attack
    /// </summary>
    protected virtual IEnumerator Cancel()
    {
        // Stop the current attack routine
        if (currentRoutine != null)
        {
            DisableIndicators();
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }

        // Destroy all fired projectiles
        foreach (var attack in spawnedProjectiles.ToArray())
        {
            if(attack != null)
                attack.Inturrupt();
        }
            

        spawnedProjectiles.Clear();
        spawnedProjectiles.TrimExcess();

        // Tell to return to base after delay
        yield return StartCoroutine(ReturnToBase(phaseChangeDuration));

        state = AttackState.Ready;
    }

    #endregion
    
    #region Utility Functions

    protected void RotateToTarget(Vector3 targetPos, float rotSpeed)
    {
        // Get target rotation
        Quaternion currRot = transform.rotation;
        Quaternion targetRot = Quaternion.LookRotation(targetPos - shootPoint.position);

        // Adjust rotation based on timescale
        float maxRot = rotSpeed * Timescale * speedModifier;

        // Apply rotation with it clamped
        if (maxRot > 0 && state != AttackState.Stunned)
            transform.rotation = Quaternion.RotateTowards(currRot, targetRot, maxRot);
    }

    /// <summary>
    /// Check if the current target has been acquired
    /// </summary>
    /// <param name="targetPos">target position to look at</param>
    /// <param name="accuracy">how accurate it should be</param>
    /// <returns>Whether target position is within accuracy tolerance</returns>
    protected bool AcquiredTarget(Vector3 targetPos, float accuracy)
    {
        // Don't acquire a target in timestop or stunned
        if (Timescale <= TimeManager.TimeStopThreshold || state == AttackState.Stunned) return false;

        // Calculate DOT product for vision, check
        float dot = Vector3.Dot(shootPoint.forward, (targetPos - shootPoint.position).normalized);
        return dot >= accuracy;
    }

    /// <summary>
    /// Coroutine that returns weapon to resting position after a delay
    /// </summary>
    /// <param name="delay">How long to wait before returning</param>
    /// <returns></returns>
    protected IEnumerator ReturnToBase(float delay)
    {
        // process delay, if available
        if (delay > 0)
        {
            SetTracker(delay);
            yield return new WaitUntil(tracker.TimerDone);
        }

        // Return to resting position
        while (!AcquiredTarget(restingPosition, 1f))
        {
            RotateToTarget(restingPosition, recoveryRotationSpeed);
            yield return null;
        }
    }

    protected void SetTracker(float delay)
    {
        float invertedSpeedMod = 1 - (speedModifier - 1);
        tracker.ResetTimer(delay * invertedSpeedMod);
    }

    #endregion

    public void OnStop()
    {
        originalPosVector = target.Center.position - transform.position;
    }

    public void OnResume()
    {
        if(state == AttackState.Attacking)
        {
            Vector3 currPos = target.Center.position - transform.position;
            float dot = Vector3.Dot(currPos.normalized, originalPosVector.normalized);

            if(dot < lossTargetVisionCone)
            {
                state = AttackState.Stunned;
                stunnedTracker.ResetTimer();
            }
        }
    }

    protected abstract void DisableIndicators();
}
