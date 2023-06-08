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

    /// <summary>
    /// Kill the current target, modified to work with enemies
    /// </summary>
    protected override void KillTarget()
    {
        // Try telling spawn manager to destroy self, if needed
        if (SpawnManager.instance != null)
        {
            SpawnManager.instance.DestroyEnemy();
            // Debug.Log($"Target {name} told manager to die!");
        }
            

        base.KillTarget();
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

    #region Knockback AI

    /// <summary>
    /// reference to navmesh agent
    /// </summary>
    private NavMeshAgent _agentRef;
    /// <summary>
    /// reference to enemy manager 
    /// </summary>
    private EnemyManager _managerRef;

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

    /// <summary>
    /// Remain in a knockback duration until landing
    /// </summary>
    /// <param name="dur">minimum duration to remain in this state</param>
    /// <returns></returns>
    private IEnumerator KnockbackDuration(float dur)
    {
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
        //knockdownTracker.ResetTimer(0.3f);

        //Vector3 ogRot = transform.rotation.eulerAngles;
        //Vector3 rot = transform.rotation.eulerAngles;
        //while (!knockdownTracker.TimerDone())
        //{
        //    rot.x = Mathf.LerpAngle(ogRot.x, 0, knockdownTracker.TimerProgress());
        //    rot.z = Mathf.LerpAngle(ogRot.z, 0, knockdownTracker.TimerProgress());
        //    transform.rotation = Quaternion.Euler(rot);
        //    yield return null;
        //}

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
    /// <summary>
    /// Core data used by enemy
    /// </summary>
    private EnemySO enemyData;

    public void PoolInit()
    {
        //Awake();
    }

    /// <summary>
    /// Return this object to its pool
    /// </summary>
    public void PoolPush()
    {
        ResetTarget();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Retrieve from pool
    /// </summary>
    public void PoolPull()
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
