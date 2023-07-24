/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - February 2nd, 2022
 * Last Edited - February 2nd, 2022 by Ben Schuster
 * Description - Controls concrete enemy-based targets
 * ================================================================================================
 */
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class EnemyTarget : Target, TimeObserver, IPoolable
{
    /// <summary>
    /// Get necessary data for knockback stuff
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        _agentRef= GetComponent<NavMeshAgent>();
        _managerRef= GetComponent<EnemyManager>();
        groundDist = Vector3.Distance(transform.position, _center.position) + 0.2f;
    }


    #region Death State

    [Header("Death State")]

    [Tooltip("Range of time the enemy stays in the death state")]
    [SerializeField] Vector2 deathStateDuration;
    [Tooltip("Minimum damage to take in a short time to go into death state instead of instantly dying")]
    [SerializeField] float deathStateDamageThreshold;
    [Tooltip("For each damage taken in frame entering death state, how much horizontal knockback is applied")]
    [SerializeField] float deathStateHorKnockback;
    [Tooltip("For each damage taken in frame entering death state, how much vertical knockback is applied")]
    [SerializeField] float deathStateVerKnockback;
    /// <summary>
    /// Timer tracking death state
    /// </summary>
    LocalTimer deathTimer;

    /// <summary>
    /// Kill the current target, modified to work with enemies
    /// </summary>
    protected override void KillTarget()
    {
        StartCoroutine(DeathState());
    }
    
    /// <summary>
    /// whether this target can be affected by attacks.
    /// doesnt include killed stats for knockback reasons
    /// </summary>
    /// <returns>whether this target enemy can be damaged</returns>
    protected override bool AffectedByAttacks()
    {
        if (invincibilityShield != null && invincibilityShield.isActiveAndEnabled)
            return false;

        return gameObject.activeInHierarchy;
    }

    /// <summary>
    /// Do a knockback state before dying
    /// </summary>
    /// <returns></returns>
    protected IEnumerator DeathState()
    {
        // drop before being launched
        _managerRef.HaltAI();

        DropAllObjs();
        yield return new WaitForEndOfFrame();

        if(damageLastFrame >= deathStateDamageThreshold)
        {
            bool originalKnockbackVal = immuneToKnockback;
            bool originalKinematic = _rb.isKinematic;

            immuneToKnockback = false;
            _rb.isKinematic = false;

            inKnockbackState = true;
            DisableAI();
            base.Knockback(damageLastFrame * deathStateHorKnockback, damageLastFrame * deathStateVerKnockback, lastDamageOrigin);
            float dur = Random.Range(deathStateDuration.x, deathStateDuration.y);

            // prepare timer
            if (deathTimer == null)
                deathTimer = GetTimer(dur);
            else
                deathTimer.ResetTimer(dur);

            // go for the entire death effect
            yield return new WaitUntil(deathTimer.TimerDone);

            immuneToKnockback = originalKnockbackVal;
            _rb.isKinematic = originalKinematic;
        }


        // Try telling spawn manager to destroy self, if needed
        if (SpawnManager.instance != null)
        {
            SpawnManager.instance.DestroyEnemy();
            // Debug.Log($"Target {name} told manager to die!");
        }

        // do full death effects
        DeathEffects();
        DestroyObject();
    }

    /// <summary>
    /// Return enemy to pool, if available, instead of destroying it
    /// </summary>
    protected override void DestroyObject()
    {
        if (EnemyPooler.instance != null)
            EnemyPooler.instance.Return(gameObject);
        else
            Destroy(gameObject);
    }

    #endregion

    #region Knockback AI
    
    /// <summary>
    /// reference to navmesh agent
    /// </summary>
    private NavMeshAgent _agentRef;
    /// <summary>
    /// reference to enemy manager 
    /// </summary>
    private EnemyManager _managerRef;

    [Header("Knockback")]

    [ShowIf("@this.immuneToKnockback == false")]
    [SerializeField] private float minKnockbackDuration = 0.5f;
    [ShowIf("@this.immuneToKnockback == false")]
    [SerializeField] private LayerMask groundMask;
    private float groundDist;

    [SerializeField] private float onGroundTime = 1f;

    private bool inKnockbackState;
    private Vector3 storedVelocity;

    private Coroutine knockbackRoutine;
    private LocalTimer knockdownTracker;

    /// <summary>
    /// Apply knockback, set up AI
    /// </summary>
    /// <param name="force">force to apply on target</param>
    /// <param name="verticalForce">Additional vertical force to apply</param>
    /// <param name="origin">Origin of the knockback</param>
    public override void Knockback(float force, float verticalForce, Vector3 origin)
    {
        if (immuneToKnockback || !AffectedByAttacks() || _killed || !gameObject.activeInHierarchy)
            return;

        try
        {
            if(!inKnockbackState)
                knockbackRoutine = StartCoroutine(KnockbackDuration(minKnockbackDuration));

            base.Knockback(force, verticalForce, origin);
        }
        catch { }
    }

    private Quaternion originalRotation;

    /// <summary>
    /// Remain in a knockback duration until landing
    /// </summary>
    /// <param name="dur">minimum duration to remain in this state</param>
    /// <returns></returns>
    private IEnumerator KnockbackDuration(float dur)
    {
        originalRotation = transform.rotation;
        inKnockbackState = true;

        DisableAI();

        if (knockdownTracker == null)
            knockdownTracker = GetTimer(dur);
        else
            knockdownTracker.ResetTimer(dur);

        // start minimum knockback duration while enemy lifts up
        yield return new WaitUntil(knockdownTracker.TimerDone);

        // Wait until enemy returns to ground
        while (true)
        {
            if (TimeManager.TimeStopped)
            {
                yield return new WaitForFixedUpdate();
                continue;
            }
            else if (LandedOnGround())
                break;

            yield return null;
        }

        knockdownTracker.ResetTimer(onGroundTime);
        yield return new WaitUntil(knockdownTracker.TimerDone);

        // rotate back upright
        //_rb.constraints = RigidbodyConstraints.FreezeAll;
        knockdownTracker.ResetTimer(1f);
        _rb.constraints = RigidbodyConstraints.FreezeAll;

        RaycastHit data;
        NavMeshHit navmeshPoint;
        Physics.Raycast(_center.position, Vector3.down,out data, Mathf.Infinity, groundMask);
        Vector3 targetPos = data.point;
        NavMesh.SamplePosition(targetPos, out navmeshPoint, Mathf.Infinity, _agentRef.areaMask);

        while (!knockdownTracker.TimerDone())
        {
            transform.position = Vector3.Lerp(transform.position, navmeshPoint.position, knockdownTracker.TimerProgress());
            transform.rotation = Quaternion.Lerp(transform.rotation, originalRotation, knockdownTracker.TimerProgress());
            yield return null;
        }
        EnableAI();
        inKnockbackState = false;
        knockbackRoutine = null;
    }

    /// <summary>
    /// Disable the AI
    /// </summary>
    private void DisableAI()
    {
        //Debug.Log("Disable AI called");

        Inturrupt();

        //_rb.isKinematic = false;
        _rb.constraints = RigidbodyConstraints.None;
        _rb.useGravity = true;

        if (_managerRef != null)
            _managerRef.enabled = false;
        if (_agentRef != null)
            _agentRef.enabled = false;
    }
    /// <summary>
    /// Reactivate the AI
    /// </summary>
    private void EnableAI()
    {
        //Debug.Log("Enable AI called");

        //_rb.isKinematic = true;
        transform.rotation = Quaternion.identity;
        _rb.constraints = RigidbodyConstraints.FreezeAll;
        _rb.useGravity = false;

        if (_managerRef != null)
            _managerRef.enabled = true;
        if (_agentRef != null)
            _agentRef.enabled = true;
    }

    /// <summary>
    /// Inturrupt the enemy AI if possible
    /// </summary>
    public override void Inturrupt()
    {
        if (_managerRef != null)
            _managerRef.InturruptAI();
    }

    /// <summary>
    /// Check if the AI has landed on the ground
    /// </summary>
    /// <returns></returns>
    private bool LandedOnGround()
    {
        // Debug.DrawLine(_center.position, _center.position + Vector3.down * groundDist, Color.yellow);
        return Physics.Raycast(_center.position, Vector3.down, groundDist, groundMask);
    }

    /// <summary>
    /// When time is stopped (called by TimeManager), store velocity and freeze
    /// </summary>
    public override void OnStop()
    {
        if (_rb != null)
        {
            storedVelocity = _rb.velocity;
            //_rb.isKinematic = true;
            // Freeze the constraints so it stops rotating
            _rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        base.OnStop();
    }

    /// <summary>
    /// When time resumes (called by TimeManager), reapply velocity 
    /// </summary>
    public override void OnResume()
    {
        if (inKnockbackState && !immuneToKnockback && _rb != null)
        {
            //_rb.isKinematic = false;
            _rb.constraints = RigidbodyConstraints.None;
            //_rb.velocity = storedVelocity;
            _rb.AddForceAtPosition(storedVelocity, _center.position, ForceMode.Impulse);
        }

        base.OnResume();
    }

    #endregion

    #region Pooling

    public void PoolInit()
    {
        //Awake();
    }

    /// <summary>
    /// Return this object to its pool
    /// </summary>
    public virtual void PoolPush()
    {
        ResetTarget();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Retrieve from pool
    /// </summary>
    public virtual void PoolPull()
    {
        // TODO - get data from difficulty scaler
        _healthManager.Heal(9999f);
    }

    /// <summary>
    /// On top of base things, reset AI so it doesnt break on respawn
    /// </summary>
    public override void ResetTarget()
    {
        base.ResetTarget();

        EnableAI();

        storedVelocity = Vector3.zero;
        OnResume();

        inKnockbackState = false;

        if (knockbackRoutine != null)
        {
            StopCoroutine(knockbackRoutine);
            knockbackRoutine = null;
        }
    }

    #endregion

    #region Cheats
    /// <summary>
    /// Input stuff for debug kill command
    /// </summary>
    private GameControls controls;
    private InputAction endEncounter;

    /// <summary>
    /// Get reference to the debug kill cheat
    /// </summary>
    protected override void OnEnable()
    {
        if(controls == null)
        {
            controls = GameManager.controls;
        }
        if(endEncounter == null && controls != null)
        {
            endEncounter = controls.PlayerGameplay.ClearEncounter;
        }
        if(endEncounter!= null)
        {
            endEncounter.performed += DebugKill;
            endEncounter.Enable();
        }

        base.OnEnable();
    }

    protected override void OnDisable()
    {
        // remove cheat to prevent bugs
        if(endEncounter != null)
            endEncounter.performed -= DebugKill;

        base.OnDisable();
    }

    /// <summary>
    /// Cheat kill command
    /// </summary>
    /// <param name="c"></param>
    private void DebugKill(InputAction.CallbackContext c)
    {
        // Dont kill godmode enemies
        if (_healthManager.God)
        {
            return;
        }

        KillTarget();
    }

    #endregion
}
