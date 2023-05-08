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
    [SerializeField, ReadOnly] protected float speedModifier = 1;

    private Vector3 restingPosition;
    protected Target target;
    protected Rigidbody targetRB;
    protected AudioSource source;

    [Header("Inturrupt Functionality")]

    [SerializeField] protected float lossTargetDistanceRequirement;
    [SerializeField] protected float lossTargetStunDuration;
    [SerializeField] protected bool lossTargetInturruptAttack;
    [SerializeField] private IIndicator[] inturruptIndicator;
    [SerializeField] private IIndicator reacquireSFX;

    [Header("Attack State")]

    [SerializeField] protected float attackRotationSpeed;
    [SerializeField] protected float attackDuration;


    [Header("Recovery State")]

    [SerializeField] protected Transform restTarget;
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
        source = GetComponent<AudioSource>();
        frame = new WaitForFixedUpdate();

        tracker = new ScaledTimer(0, Affected);
        stunnedTracker = new ScaledTimer(lossTargetStunDuration, Affected);
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
            {
                Indicators.SetIndicators(inturruptIndicator, false);
                reacquireSFX.Activate();
                state = AttackState.Attacking;
            }
                
        }
    }

    public abstract IEnumerator AttackRoutine();

    #region Virtual Attack Functions

    /// <summary>
    /// Cancel the current attack, if possible
    /// </summary>
    public virtual void CancelAttack()
    {
        // Stop the current attack routine
        if (currentRoutine != null)
        {
            DisableIndicators();
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }

        StopAllCoroutines();
        StartCoroutine(Cancel());
    }

    /// <summary>
    /// Cancel the current attack
    /// </summary>
    protected virtual IEnumerator Cancel()
    {
        // Destroy all fired projectiles
        foreach (var attack in spawnedProjectiles.ToArray())
        {
            if(attack != null)
                attack.Inturrupt();
        }
        
        spawnedProjectiles.Clear();
        spawnedProjectiles.TrimExcess();

        //Debug.Log($"{transform.parent.name} Cancel BFL attack called");
        // Tell to return to base after delay
        //yield return StartCoroutine(ReturnToBase(phaseChangeDuration));

        state = AttackState.Ready;
        yield return null;
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
        
        ScaledTimer maxTime = new ScaledTimer(1.5f);

        Vector3 rot = transform.localRotation.eulerAngles;
        // mod it to prevent it from ALWAYS GOING COUNTERCLOCKWISE
        if (rot.x > 180)
            rot.x -= 360;
        if(rot.y > 180)
            rot.y -= 360;
        if (rot.z > 180)
            rot.z -= 360;

        // Return to resting position
        while (!maxTime.TimerDone())
        {
            transform.localRotation = Quaternion.Euler(Vector3.Lerp(rot, Vector3.zero, maxTime.TimerProgress()));
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
        originalPosVector = target.Center.position;
        BonusStop();
    }

    public void OnResume()
    {
        if(state == AttackState.Attacking)
        {
            float dist = Vector3.Distance(target.Center.position, originalPosVector);

            if (dist > lossTargetDistanceRequirement)
            {
                state = AttackState.Stunned;
                stunnedTracker.ResetTimer();
                Indicators.SetIndicators(inturruptIndicator, true);
                BonusResume();
            }
        }
    }

    protected virtual void BonusStop()
    {
        return;
    }
    protected virtual void BonusResume()
    {
        return;
    }

    protected abstract void DisableIndicators();

    public void RotateToDisabledState()
    {
        StartCoroutine(RotateToDisable());
    }
    public void RotateToEnabledState()
    {
        //Debug.Log($"{transform.parent.name} Rotate to enabled state called");
        StartCoroutine(ReturnToBase(0));
    }
    protected IEnumerator RotateToDisable()
    {
        Vector3 tar = new Vector3(90, 0, 0);
        ScaledTimer maxTime = new ScaledTimer(1.5f);
        Vector3 rot = transform.localRotation.eulerAngles;
        // mod it to prevent it from ALWAYS GOING COUNTERCLOCKWISE
        if (rot.x > 180)
            rot.x -= 360;
        if (rot.y > 180)
            rot.y -= 360;
        if (rot.z > 180)
            rot.z -= 360;


        // Return to resting position
        while (!maxTime.TimerDone())
        {
            transform.localRotation = Quaternion.Euler(Vector3.Lerp(rot, tar, maxTime.TimerProgress()));
            yield return null;
        }
    }
}
