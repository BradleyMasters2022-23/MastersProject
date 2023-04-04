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

public class EnemyTarget : Target, TimeObserver
{
    #region Pooling
    /// <summary>
    /// Core data used by enemy
    /// </summary>
    private EnemySO enemyData;

    /// <summary>
    /// Return this object to its pool
    /// </summary>
    public void ReturnToPool()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Retrieve from pool
    /// </summary>
    public void PullFromPool(EnemySO data)
    {
        if (enemyData == null)
            enemyData = data;

        _killed = false;
        _healthManager.ResetHealth();
        // gameObject.SetActive(true);
        // TODO - get data from difficulty scaler
    }
    #endregion

    private NavMeshAgent _agentRef;
    private EnemyManager _managerRef;

    [ShowIf("@this.immuneToKnockback == false")]
    [SerializeField] private float minKnockbackDuration = 0.5f;
    [ShowIf("@this.immuneToKnockback == false")]
    [SerializeField] private float groundDist;
    [ShowIf("@this.immuneToKnockback == false")]
    [SerializeField] private LayerMask groundMask;

    private bool inKnockbackState;
    private Vector3 storedVelocity;

    protected override void Awake()
    {
        base.Awake();
        _agentRef= GetComponent<NavMeshAgent>();
        _managerRef= GetComponent<EnemyManager>();
    }

    /// <summary>
    /// Kill the current target, modified to work with enemies
    /// </summary>
    protected override void KillTarget()
    {
        // Try telling spawn manager to destroy self, if needed
        if (SpawnManager.instance != null)
            SpawnManager.instance.DestroyEnemy();

        base.KillTarget();
    }

    public override void Knockback(float force, float verticalForce, Vector3 origin)
    {
        if (immuneToKnockback)
            return;

        StartCoroutine(KnockbackDuration(minKnockbackDuration));
        base.Knockback(force, verticalForce, origin);
    }

    private IEnumerator KnockbackDuration(float dur)
    {
        inKnockbackState = true;

        DisableAI();

        // start minimum knockback duration while enemy lifts up
        ScaledTimer tracker = new ScaledTimer(dur);
        yield return new WaitUntil(() => tracker.TimerDone());

        // Wait until enemy returns to ground
        while(true)
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


        EnableAI();
        inKnockbackState = false;
    }
    private void DisableAI()
    {
        Inturrupt();

        _rb.isKinematic = false;
        _rb.useGravity = true;

        if (_managerRef != null)
            _managerRef.enabled= false;
        if(_agentRef != null)
            _agentRef.enabled = false;
    }

    private void EnableAI()
    {
        _rb.isKinematic = true;
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
        if(_managerRef != null)
            _managerRef.InturruptAI();
    }

    private bool LandedOnGround()
    {
        return Physics.CheckSphere(_center.position, groundDist, groundMask);
    }

    /// <summary>
    /// Return enemy to pool, if available, instead of destroying it
    /// </summary>
    protected override void DestroyObject()
    {
        if(EnemyPooler.instance != null)
            EnemyPooler.instance.Return(enemyData, gameObject);
        else
            Destroy(gameObject);
    }

    public void OnStop()
    {
        if (_rb != null)
        {
            storedVelocity = _rb.velocity;
            _rb.isKinematic = true;
        }

    }

    public void OnResume()
    {
        if (inKnockbackState && !immuneToKnockback && _rb != null)
        {
            _rb.isKinematic = false;
            _rb.velocity = storedVelocity;
        }
    }

    #region Cheats
    /// <summary>
    /// Input stuff for debug kill command
    /// </summary>
    private GameControls controls;
    private InputAction endEncounter;

    /// <summary>
    /// Get reference to the debug kill cheat
    /// </summary>
    private void OnEnable()
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

        TimeManager.instance.Subscribe(this);
    }

    private void OnDisable()
    {
        // remove cheat to prevent bugs
        if(endEncounter != null)
            endEncounter.performed -= DebugKill;

        TimeManager.instance.UnSubscribe(this);
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
