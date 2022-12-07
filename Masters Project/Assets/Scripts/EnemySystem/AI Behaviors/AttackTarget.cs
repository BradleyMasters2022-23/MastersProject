/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - December 5, 2022
 * Last Edited - December 5, 2022 by Ben Schuster
 * Description - Base enemy attack behavior that contains the main routine for attacking
 * ================================================================================================
 */
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public enum AttackState
{
    Ready,
    Indicator,
    Damaging,
    Finishing, 
    Cooldown
}

public abstract class AttackTarget : MonoBehaviour
{
    [Header("=== Core Attack Data ===")]

    public AttackState currentAttackState;

    //[Tooltip("Whether or not the animator is responsible for this attack system")]
    //[SerializeField] protected bool animationControlled;

    /// <summary>
    /// animator for this enemy
    /// </summary>
    protected Animator animator;

    [Tooltip("Whether or not the attack is ready")]
    [HideInInspector] public bool attackReady;

    [Tooltip("Length of time between attack actions")]
    [SerializeField] protected float attackCoolown;

    [Space(5)]
    [Tooltip("How long indicator lasts before attacking")]
    [SerializeField] protected float indicatorDuration;
    [Tooltip("Whether the enemy can rotate during the indicator stage")]
    [SerializeField] protected bool rotateDuringIndication;

    [Space(5)]
    [Tooltip("How long the enemy stays in the finished attack state")]
    [SerializeField] protected float finishDuration;
    [Tooltip("Whether the enemy can rotate during the finish stage")]
    [SerializeField] protected bool rotateDuringFinish;

    [Space(5)]
    [Tooltip("Rotation speed of the enemy")]
    [SerializeField][Range(0f, 5f)] protected float rotationSpeed;

    [Header("=== Testing and Debug ===")]

    [Tooltip("Whether the enemy will automatically loop its attacks. Use this to test new attacks without the manager.")]
    [SerializeField] protected bool autoLoopAttack;
    [Tooltip("The target of the attack."), HideIf("@this.autoLoopAttack == false")]
    [SerializeField] protected Transform target;
    

    #region Timer Variables

    protected ScaledTimer attackTracker;
    protected ScaledTimer indicatorTracker;
    protected ScaledTimer finishTracker;

    #endregion

    protected virtual void Awake()
    {
        Debug.Log("attack target awake called");
        attackTracker = new ScaledTimer(attackCoolown);

        indicatorTracker = new ScaledTimer(indicatorDuration);
        finishTracker = new ScaledTimer(finishDuration);
    }

    private void FixedUpdate()
    {
        StateUpdateFunctionality();

        // this is for testing
        if(autoLoopAttack && currentAttackState == AttackState.Ready)
        {
            Attack(target);
        }
    }

    private void StateUpdateFunctionality()
    {
        switch(currentAttackState)
        {
            case AttackState.Ready:
                {
                    break;
                }
            case AttackState.Indicator:
                {
                    IndicatorUpdateFunctionality();
                    break;
                }
            case AttackState.Damaging:
                {
                    DamageUpdateFunctionality();
                    break;
                }
            case AttackState.Finishing:
                {
                    if (rotateDuringFinish)
                        RotateToTarget();

                    break;
                }
            case AttackState.Cooldown:
                {
                    if (attackTracker.TimerDone())
                    {
                        currentAttackState = AttackState.Ready;
                        attackReady = true;
                    }

                    break;
                }
        }
    }

    /// <summary>
    /// Begin the attack routine
    /// </summary>
    /// <param name="target"></param>
    public void Attack(Transform t)
    {
        if(currentAttackState == AttackState.Ready)
        {
            target = t;
            StartCoroutine(AttackAction());
        }
    }

    /// <summary>
    /// Manage the entire routine of attacking
    /// </summary>
    /// <returns></returns>
    protected IEnumerator AttackAction()
    {
        attackReady = false;

        currentAttackState = AttackState.Indicator;
        yield return StartCoroutine(AttackIndicator());

        currentAttackState = AttackState.Damaging;
        yield return StartCoroutine(DamageAction());

        currentAttackState = AttackState.Finishing;
        yield return  StartCoroutine(FinishAttack());

        attackTracker.ResetTimer();
        currentAttackState = AttackState.Cooldown;

        yield return null;
    }

    #region Indicator Stuff

    /// <summary>
    /// Manage exclusively the indicator for attacking
    /// </summary>
    /// <returns></returns>
    protected IEnumerator AttackIndicator()
    {
        // Enable indicator stuff here
        ShowIndicator();

        // Wait
        indicatorTracker.ResetTimer();
        while(!indicatorTracker.TimerDone())
        {
            // Do things while waiting (spin indicator VFX?)
            yield return null;
        }

        // Disable indicator stuff here
        HideIndicator();

        yield return null;
    }

    /// <summary>
    /// Show any indicators being used by this attack, or play any sounds
    /// </summary>
    protected virtual void ShowIndicator()
    {
        // by default, do nothing
        return;
    }
    /// <summary>
    /// Hide any indicators being used by this attack, or stop any indicators
    /// </summary>
    protected virtual void HideIndicator()
    {
        // by default, do nothing 
        return;
    }
    /// <summary>
    /// perform in update during indicator phase. Always look at target by default
    /// </summary>
    protected virtual void IndicatorUpdateFunctionality()
    {
        // by default, track normally. Can edit later
        if (rotateDuringIndication)
            RotateToTarget();
        return;
    }

    #endregion

    /// <summary>
    /// Perform the action that deals damage, whatever that may be
    /// </summary>
    /// <returns></returns>
    protected abstract IEnumerator DamageAction();
    /// <summary>
    /// Perform in update during damage phase. Do nothing by default
    /// </summary>
    protected virtual void DamageUpdateFunctionality()
    {
        return;
    }

    #region Attack Finished Stuff

    /// <summary>
    /// Manage exclusively what happens to the enemy immediately after attacking, such as recoil
    /// </summary>
    /// <returns></returns>
    protected IEnumerator FinishAttack()
    {
        // Enable delay stuff here
        ShowAttackDone();

        // Wait
        finishTracker.ResetTimer();
        while (!finishTracker.TimerDone())
        {
            // Do things while waiting (spin indicator VFX?)
            yield return null;
        }

        // Disable delay stuff here
        HideAttackDone();

        yield return null;
    }
    /// <summary>
    /// Do anything when the enemy's finished attack completes.
    /// </summary>
    protected virtual void ShowAttackDone()
    {
        // Nothing by default
        return;
    }
    /// <summary>
    /// Do anything when the enemy's recoil is done
    /// </summary>
    protected virtual void HideAttackDone()
    {
        // Nothing by default
        return;
    }

    #endregion

    /// <summary>
    /// Rotate to the target you're attacking. can be overriden for special rotation needs such as turrets
    /// </summary>
    protected virtual void RotateToTarget()
    {
        RotateToTarget(target.position);
    }

    /// <summary>
    /// Prepare to look at the target, but add a modifier
    /// </summary>
    /// <param name="modifier">Overrider modifier</param>
    protected virtual void RotateToTarget(Vector3 targetPos)
    {
        Vector3 direction;
        direction = ((targetPos) - transform.position);

        // rotate towards them, clamped
        Quaternion rot = Quaternion.LookRotation(direction);
        float nextYAng = Mathf.Clamp(Mathf.DeltaAngle(gameObject.transform.rotation.eulerAngles.y, rot.eulerAngles.y),
            -rotationSpeed, rotationSpeed) * TimeManager.WorldTimeScale;
        float nextXAng = Mathf.Clamp(Mathf.DeltaAngle(gameObject.transform.rotation.eulerAngles.x, rot.eulerAngles.x),
            -rotationSpeed, rotationSpeed) * TimeManager.WorldTimeScale;

        transform.rotation = Quaternion.Euler(
            gameObject.transform.rotation.eulerAngles.x + nextXAng,
            gameObject.transform.rotation.eulerAngles.y + nextYAng,
            0);
    }
}
