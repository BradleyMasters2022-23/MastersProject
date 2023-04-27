/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - April 26, 2022
 * Last Edited - April 26, 2022 by Ben Schuster
 * Description - Manages boss specific attack behaviors. Done to accomodate new manager
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossAttack : AttackTarget
{
    BossManager boss;

    protected override void Awake()
    {
        boss = GetComponent<BossManager>();
        affected = boss.GetAfflicted();
        source = GetComponent<AudioSource>();
        attackTracker = new ScaledTimer(attackCoolown, false);
        indicatorTracker = new ScaledTimer(indicatorDuration, false);
        finishTracker = new ScaledTimer(finishDuration, false);

        if (lineOfSightOrigin == null)
            lineOfSightOrigin = transform;

        currentAttackState = AttackState.Ready;

        UpdateTime();
    }

    protected override void UpdateTime()
    {
        affected = boss.GetAfflicted();
        timeScale = boss.GetTimescale();
        deltaTime = boss.GetDeltatime();

        attackTracker?.SetModifier(timeScale);
        indicatorTracker?.SetModifier(timeScale);
        finishTracker?.SetModifier(timeScale);
    }

    public override bool CanDoAttack()
    {
        return (currentAttackState == AttackState.Ready);
    }
}
