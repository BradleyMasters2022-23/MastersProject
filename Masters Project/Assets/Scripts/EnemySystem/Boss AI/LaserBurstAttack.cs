using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBurstAttack : BaseBossAttack
{
    [Header("Laser Burst Attack")]

    [SerializeField, Range(0, 1)] private float accuracy;
    [SerializeField] private float minimumAimTime;

    [SerializeField] private IIndicator[] aimingIndicator;
    [SerializeField] private IIndicator[] firingIndicator;
    [SerializeField] private float fireDelay;
    [SerializeField, Range(0, 100)] private int leadShotChance;

    public override IEnumerator AttackRoutine()
    {
        for(int i = 0; i < numberOfAttacks; i++)
        {
            // Determine whether or not to lead
            state = AttackState.Preparing;

            Vector3 aimPos = target.Center.position;
            float chance = Random.Range(1, 100);
            bool leadShot = chance <= leadShotChance;

            Indicators.SetIndicators(aimingIndicator, true);

            // Continually look at target until acquired and minimum aim time done
            state = AttackState.Attacking;
            TimeManager.instance.Subscribe(this);
            tracker.ResetTimer(minimumAimTime);

            while (!tracker.TimerDone())
            {
                if (leadShot)
                    aimPos = target.Center.position + (targetRB.velocity * fireDelay);
                else
                    aimPos = target.Center.position;

                RotateToTarget(aimPos, attackRotationSpeed);
                yield return frame;
            }

            // Stay on the point for delay and firing
            Indicators.SetIndicators(firingIndicator, true);
            tracker.ResetTimer(fireDelay);
            yield return new WaitUntil(tracker.TimerDone);

            TimeManager.instance.UnSubscribe(this);

            // Fire the attack, remained focused on position for attack
            RangeAttack laser = Instantiate(projectilePrefab, shootPoint).GetComponent<Laser>();
            laser.Initialize(1, 1, attackDuration, shootPoint.gameObject);
            spawnedProjectiles.Add(laser);

            // Remained focused on the attack during duration
            tracker.ResetTimer(attackDuration);
            yield return new WaitUntil(tracker.TimerDone);

            // Recover from the attack
            state = AttackState.Recovering;
            Indicators.SetIndicators(firingIndicator, false);
            Indicators.SetIndicators(aimingIndicator, false);
            tracker.ResetTimer(recoveryDuration);
            yield return new WaitUntil(tracker.TimerDone);
        }

        yield return StartCoroutine(ReturnToBase(recoveryDuration));

        // Initialize cooldown
        state = AttackState.Cooldown;

        float cd = Random.Range(cooldownRange.x, cooldownRange.y);
        SetTracker(cd);
        yield return new WaitUntil(tracker.TimerDone);

        state = AttackState.Ready;
        currentRoutine = null;

        yield return null;
    }

    protected override void DisableIndicators()
    {
        Indicators.SetIndicators(firingIndicator, false);
        Indicators.SetIndicators(aimingIndicator, false);
    }
}
