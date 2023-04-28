using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseBeamAttack : BaseBossAttack
{
    [Header("Chase Beam Attack")]

    [SerializeField] private Transform initialAimPosition;
    [SerializeField] private float initialFireDuration;

    public override IEnumerator AttackRoutine()
    {
        for(int i = 0; i < numberOfAttacks; i++)
        {
            state = AttackState.Preparing;

            // Aim at initial starting position
            Vector3 aimPos = initialAimPosition.position;
            while (!AcquiredTarget(aimPos, 1))
            {
                RotateToTarget(aimPos, attackRotationSpeed);
                yield return frame;
                yield return null;
            }

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
                Debug.Log("Tracker progress at " + tracker.TimerProgress());
                aimPos = target.Center.position;
                RotateToTarget(aimPos, attackRotationSpeed);
                yield return frame;
                yield return null;
            }

            TimeManager.instance.UnSubscribe(this);

            // Wait for recovery stationary
            state = AttackState.Recovering;
            yield return StartCoroutine(ReturnToBase(recoveryDuration));
        }

        // Initialize cooldown
        state = AttackState.Cooldown;

        float cd = Random.Range(cooldownRange.x, cooldownRange.y);
        SetTracker(cd);
        yield return new WaitUntil(tracker.TimerDone);

        state = AttackState.Ready;
        currentRoutine = null;
    }
}
