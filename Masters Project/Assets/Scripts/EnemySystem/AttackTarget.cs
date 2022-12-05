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
    public AttackState currentAttackState;

    [Tooltip("Whether or not the animator is responsible for this attack system")]
    [SerializeField] protected bool animationControlled;

    /// <summary>
    /// animator for this enemy
    /// </summary>
    protected Animator animator;

    /// <summary>
    /// whether or not the enemy is currently attacking
    /// </summary>
    private bool attacking;

    [Tooltip("Whether or not the attack is ready")]
    public bool attackReady;

    [Tooltip("Length of time between attack actions")]
    [SerializeField] protected float attackCoolown;

    [Tooltip("How long indicator lasts before attacking")]
    [SerializeField] protected float indicatorDuration;
    [Tooltip("How long the enemy stays in the finished attack state")]
    [SerializeField] protected float finishDuration;

    #region Timer Variables

    protected ScaledTimer attackTracker;
    protected ScaledTimer indicatorTracker;
    protected ScaledTimer finishTracker;

    #endregion

    private void Awake()
    {
        attackTracker = new ScaledTimer(attackCoolown);

        indicatorTracker = new ScaledTimer(indicatorDuration);
        finishTracker = new ScaledTimer(finishDuration);
    }

    private void Update()
    {
        if(attackTracker.TimerDone() && !attacking)
        {
            currentAttackState= AttackState.Ready;
        }

        // if not alrady attacking and the timer is done, it can attack
        if(attackTracker.TimerDone() && !attacking)
        {
            StartCoroutine(AttackAction());
        }
    }

    /// <summary>
    /// Manage the entire routine of attacking
    /// </summary>
    /// <returns></returns>
    protected IEnumerator AttackAction()
    {
        attacking = true;

        currentAttackState = AttackState.Indicator;
        yield return StartCoroutine(AttackIndicator());

        currentAttackState = AttackState.Damaging;
        yield return StartCoroutine(DamageAction());

        currentAttackState = AttackState.Finishing;
        yield return StartCoroutine(FinishAttack());

        attacking = false;
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
        while(indicatorTracker.TimerDone())
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

    #endregion

    /// <summary>
    /// Perform the action that deals damage, whatever that may be
    /// </summary>
    /// <returns></returns>
    protected abstract IEnumerator DamageAction();

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
        while (finishTracker.TimerDone())
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
}
