using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseBeamAttack : BaseBossAttack
{
    [Header("Chase Beam Attack")]

    [SerializeField] private Transform initialAimPosition;
    [SerializeField] private float preFireDuration;
    [SerializeField] private float initialFireDuration;
    

    [SerializeField] private float windupRotationSpeed;
    [SerializeField] private IIndicator[] chargingIndicators;
    [SerializeField] private IIndicator[] firingIndicators;
    [SerializeField] private IIndicator[] finishIndicators;

    public override IEnumerator AttackRoutine()
    {
        for(int i = 0; i < numberOfAttacks; i++)
        {
            state = AttackState.Preparing;

            Indicators.SetIndicators(chargingIndicators, true);

            // Aim at initial starting position
            Vector3 aimPos = initialAimPosition.position;
            while (!AcquiredTarget(aimPos, 1))
            {
                RotateToTarget(aimPos, windupRotationSpeed);
                yield return frame;
                yield return null;
            }

            // aim at position for a short time
            tracker.ResetTimer(preFireDuration);
            yield return new WaitUntil(tracker.TimerDone);

            Indicators.SetIndicators(chargingIndicators, false);

            Indicators.SetIndicators(firingIndicators, true);

            // Fire laser and initialize
            RangeAttack laser = Instantiate(projectilePrefab, shootPoint).GetComponent<Laser>();
            laser.Initialize(1, 1, (initialFireDuration + attackDuration), shootPoint.gameObject);
            spawnedProjectiles.Add(laser);

            // Remain firing at ground for specified duration
            tracker.ResetTimer(initialFireDuration);
            yield return new WaitUntil(tracker.TimerDone);

            // Begin chasing player
            state = AttackState.Attacking;
            TimeManager.instance.Subscribe(this);

            //Debug.Log($"Resetting tracker to : {attackDuration}");
            tracker.ResetTimer(attackDuration);
            while (!tracker.TimerDone())
            {
                aimPos = target.Center.position;
                RotateToTarget(aimPos, attackRotationSpeed);
                yield return frame;
                yield return null;
            }

            TimeManager.instance.UnSubscribe(this);
            Indicators.SetIndicators(firingIndicators, false);

            Indicators.SetIndicators(finishIndicators, true);

            // Wait for recovery stationary
            state = AttackState.Recovering;
            yield return StartCoroutine(ReturnToBase(recoveryDuration));

            Indicators.SetIndicators(finishIndicators, false);
        }

        // Initialize cooldown
        state = AttackState.Cooldown;

        float cd = Random.Range(cooldownRange.x, cooldownRange.y);
        SetTracker(cd);
        yield return new WaitUntil(tracker.TimerDone);

        state = AttackState.Ready;
        currentRoutine = null;
    }

    protected override void DisableIndicators()
    {
        Indicators.SetIndicators(chargingIndicators, false);
        Indicators.SetIndicators(firingIndicators, false);
        Indicators.SetIndicators(finishIndicators, false);
    }
}
