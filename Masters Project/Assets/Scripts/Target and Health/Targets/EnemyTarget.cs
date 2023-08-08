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
using UnityEngine.VFX;

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
        defaultLookAngle = flinchRoot.localRotation;

        originalKinematicSetting = _rb.isKinematic;
        originalKnockbackImmunitySetting = immuneToKnockback;
    }

    #region Flinching

    [Header("Flinching")]

    [Tooltip("Max angle that can be applied due to flinch in any direction")]
    [SerializeField] float maxFlinch;
    [Tooltip("Transform container holding the main enemy body")]
    [SerializeField] Transform flinchRoot;

    [Tooltip("How fast the enemy returns to normal orientation after being hit by flinch. Higher this is, the faster they recover.")]
    [SerializeField] float flinchRecoveryRate = 3;
    [Tooltip("For each point of damage taken, how much flinch is applied")]
    [SerializeField] float flinchSensitivity = 1;
    /// <summary>
    /// Default look angle for the enemy flinch root
    /// </summary>
    private Quaternion defaultLookAngle;
    /// <summary>
    /// Current angle applied to the flinch root
    /// </summary>
    private float currentFlinchAngle;

    /// <summary>
    /// After taking damage, apply flinch
    /// </summary>
    /// <param name="dmg"></param>
    /// <param name="origin"></param>
    public override void RegisterEffect(float dmg, Vector3 origin)
    {
        base.RegisterEffect(dmg, origin);


        Flinch(dmg, origin);
    }

    /// <summary>
    /// Apply flinch to the enemy model. Does not work if in a knockback state
    /// </summary>
    /// <param name="magnitude">Magnitude of the flinch to apply. Stacks.</param>
    /// <param name="origin">Origin of the flinch source</param>
    private void Flinch(float magnitude, Vector3 origin)
    {
        // don't flinch if already in knockback
        if (inKnockbackState) return;

        // calculate cross product for perpendicular axis
        Vector3 dir = flinchRoot.position - origin;
        Vector3 newAxis = Vector3.Cross(dir, _center.up);

        // invert based on height so its properly vertical
        float invert = (dir.y <= 0) ? -1 : 1;

        // calculate angle. Clamp it in a way that adjusts current flinch angle properly
        float amount = magnitude * flinchSensitivity * invert;
        if(currentFlinchAngle + amount <= -maxFlinch)
        {
            amount = -maxFlinch - currentFlinchAngle;
            currentFlinchAngle = -maxFlinch;

        }
        else if(currentFlinchAngle + amount >= maxFlinch)
        {
            amount = maxFlinch - currentFlinchAngle;
            currentFlinchAngle = maxFlinch;
        }
        else
        {
            currentFlinchAngle += amount;
        }

        // apply
        flinchRoot.Rotate(newAxis, amount, Space.World);
    }

    /// <summary>
    /// Continually attempt to recover from the flinch
    /// </summary>
    protected override void Update()
    {
        base.Update();

        if(flinchRoot.localRotation != defaultLookAngle && !Slowed)
        {
            flinchRoot.localRotation = Quaternion.Lerp(flinchRoot.localRotation, defaultLookAngle, flinchRecoveryRate * DeltaTime);
            currentFlinchAngle = Mathf.Lerp(currentFlinchAngle, 0, flinchRecoveryRate * DeltaTime);

            // if now upright, refresh current flinch angle
            if (flinchRoot.localRotation == defaultLookAngle)
                currentFlinchAngle = 0;
        }
    }

    #endregion

    #region Death State

    [Header("Death State")]

    [Tooltip("Animator controlling this enemy's death state")]
    [SerializeField] Animator deathAnimator;
    [Tooltip("Range of time the enemy stays in the death state")]
    [SerializeField] Vector2 deathSpeedMinMax;
    [Tooltip("Speed modifier applied to the enemy when entering the death fling state")]
    [SerializeField] float deathFlingSpeedMod;
    [Tooltip("If the enemy dies on the spot, how much vertical force is applied")]
    [SerializeField] float instantDeathPopForce;
    [Tooltip("Minimum damage to take in a short time to go into death state instead of instantly dying")]
    [SerializeField] float deathStateDamageThreshold;
    [Tooltip("For each damage taken in frame entering death state, how much horizontal knockback is applied")]
    [SerializeField] float deathStateHorKnockback;
    [Tooltip("For each damage taken in frame entering death state, how much vertical knockback is applied")]
    [SerializeField] float deathStateVerKnockback;
    [Tooltip("On collision with an object at this velocity, detonate instantly")]
    [SerializeField] float instantImpactThreshold;

    [Tooltip("SFX that plays when entering the death state")]
    [SerializeField] AudioClipSO deathStateSFX;
    /// <summary>
    /// Whether the enemy is currently in a death state
    /// </summary>
    private bool inDeathState;

    /// <summary>
    /// On collision in death state, immediately detondate
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionEnter(Collision collision)
    {
        if (inDeathState && _rb.velocity.magnitude >= instantImpactThreshold)
        {
            StopCoroutine(DeathState());
            DestroyEnemy();
        }
        else if (inKnockbackState && _rb.velocity.magnitude >= impactSoundVelocity)
        {
            impactSoundSFX.PlayClip(audioSource, true);
        }
    }

    /// <summary>
    /// original isKinematic setting without any tampering
    /// </summary>
    private bool originalKinematicSetting;
    /// <summary>
    /// original knockback immunity setting without any tampering
    /// </summary>
    private bool originalKnockbackImmunitySetting;

    /// <summary>
    /// Kill the current target, modified to work with enemies
    /// </summary>
    protected override void KillTarget()
    {
        // inturrupt any acctive knockback routine
        if (knockbackRoutine != null)
        {
            StopCoroutine(knockbackRoutine);
        }

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
        if(Affected)
            yield return new WaitUntil(() => !Slowed);

        // drop before being launched
        _managerRef.HaltAI();

        DropAllObjs();
        inDeathState = true;

        yield return new WaitForEndOfFrame();
        float speedMod = Random.Range(deathSpeedMinMax.x, deathSpeedMinMax.y);

        _rb.isKinematic = false;
        immuneToKnockback = false;
        deathStateSFX.PlayClip(audioSource);

        inKnockbackState = true;
        DisableAI();

        // if enough damage for launch state, do that
        if (damageLastFrame >= deathStateDamageThreshold)
        {
            speedMod *= deathFlingSpeedMod;
            base.Knockback(damageLastFrame * deathStateHorKnockback, damageLastFrame * deathStateVerKnockback, lastDamageOrigin);
        }
        // otherwise, do a vertical pop
        else
        {
            base.Knockback(0, instantDeathPopForce, _center.position);
        }

        deathAnimator.SetFloat("SpeedMod", speedMod);
        deathAnimator.SetTrigger("Dissolve");
        DeathEffects();
        //DestroyEnemy();
    }

    protected void DestroyEnemy()
    {
        // Try telling spawn manager to destroy self, if needed
        if (SpawnManager.instance != null)
        {
            SpawnManager.instance.DestroyEnemy();
            // Debug.Log($"Target {name} told manager to die!");
        }

        _rb.isKinematic = originalKinematicSetting;
        immuneToKnockback = originalKnockbackImmunitySetting;

        // do full death effects
        
        DestroyObject();
    }

    /// <summary>
    /// Return enemy to pool, if available, instead of destroying it
    /// </summary>
    protected override void DestroyObject()
    {
        if (EnemyPooler.instance != null)
        {
            EnemyPooler.instance.Return(gameObject);

        }
        else
        {
            //Debug.Log("No pooler detected, destroying self");
            Destroy(gameObject);
        }
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
    [Tooltip("Minimum time to stay in the knockback state")]
    [SerializeField] private float minKnockbackDuration = 0.5f;
    [ShowIf("@this.immuneToKnockback == false")]
    [Tooltip("Ground layers to use for checking grounded status")]
    [SerializeField] private LayerMask groundMask;
    [Tooltip("Minimum time to remain on ground after being knocked down")]
    [SerializeField] private float onGroundTime = 1f;

    [Tooltip("SFX to play when hitting something in knockback state")]
    [SerializeField] private AudioClipSO impactSoundSFX;
    [Tooltip("Minimum velocity to play on impact sound")]
    [SerializeField] private float impactSoundVelocity;

    /// <summary>
    /// internal tracker for distance from ground
    /// </summary>
    private float groundDist;
    /// <summary>
    /// Whether currently in knockback state
    /// </summary>
    private bool inKnockbackState;
    /// <summary>
    /// The velocity currently stored
    /// </summary>
    private Vector3 storedVelocity;

    /// <summary>
    /// Routine tracker for knockback
    /// </summary>
    private Coroutine knockbackRoutine;
    /// <summary>
    /// Timer tracking knockdown state
    /// </summary>
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
            if (!inKnockbackState && !inDeathState)
            {
                // inturrupt the last knockback routine if possible
                if (knockbackRoutine != null)
                    StopCoroutine(knockbackRoutine);

                knockbackRoutine = StartCoroutine(KnockbackDuration(minKnockbackDuration));
            }
                
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

        float timePassed = 0;
        // Wait until enemy returns to ground
        while (true)
        {
            timePassed += DeltaTime;


            if (TimeManager.TimeStopped)
            {
                yield return new WaitForFixedUpdate();
                continue;
            }
            else if (LandedOnGround())
                break;
            else if(timePassed > 8f) // if stuck in knockback state for 8 seconds, die to prevent breaks
            {
                base.KillTarget();
                yield break;
            }

            yield return null;
        }

        knockdownTracker.ResetTimer(onGroundTime);
        yield return new WaitUntil(knockdownTracker.TimerDone);


        // exit knockback state during recovery, that way it can be inturrupted properly
        inKnockbackState = false;

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
        inDeathState = false;

        _rb.isKinematic = originalKinematicSetting;
        immuneToKnockback = originalKnockbackImmunitySetting;

        audioSource.Stop();

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
