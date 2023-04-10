using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class BossLaserAttack : MonoBehaviour
{
    protected enum AttackState
    {
        Idle,
        Active, 
        Cooldown
    }

    private AttackState currentState;
    private Coroutine attackRoutine;

    /// <summary>
    /// Whether or not this attack can be used
    /// </summary>
    /// <returns></returns>
    public abstract bool CanAttack();

    /// <summary>
    /// Start the attack
    /// </summary>
    public void StartAttack()
    {
        currentState = AttackState.Active;
        StartCoroutine(AttackRoutine());
    }

    protected abstract IEnumerator AttackRoutine();

    public abstract bool AttackDone();

    public virtual void Inturrupt()
    {
        if(attackRoutine != null)
        {
            attackRoutine = null;
            currentState = AttackState.Idle;
        }
    }
}
