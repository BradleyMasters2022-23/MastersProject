using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class BossCannonController : MonoBehaviour
{
    [SerializeField] private Target target;
    private Rigidbody targetRB;
    
    private ScaledTimer tracker;

    private Vector3 restingPos;

    private Coroutine currAttack;

    private void Awake()
    {
        targetRB = target.GetComponent<Rigidbody>();

        restingPos = transform.position + transform.forward*100;

        currAttack = StartCoroutine(ChaseBeamAttack());
    }

    public void RotateToTarget(Vector3 targetPos, float rotSpeed)
    {
        Quaternion currRot = transform.rotation;
        Quaternion targetRot = Quaternion.LookRotation(targetPos - shootPoint.position);

        Debug.DrawLine(targetPos, shootPoint.position, Color.green);

        float maxRot = rotSpeed * TimeManager.WorldTimeScale;

        if (maxRot > 0)
            transform.rotation = Quaternion.RotateTowards(currRot, targetRot, maxRot);
    }

    public bool AcquiredTarget(Vector3 targetPos, float accuracy)
    {
        if (TimeManager.TimeStopped)
            return false;

        float dot = Vector3.Dot(shootPoint.forward, (targetPos - shootPoint.position).normalized);

        return dot >= accuracy;
    }

    #region Focused Beam Attack

    [Header("Focused Beam Attack")]

    [SerializeField] private GameObject focusedBeamPrefab;
    [SerializeField] private Transform shootPoint;

    [SerializeField] private float fbRotationSpeed;
    [SerializeField, Range(0, 1)] private float fbAccuracy;

    [SerializeField] private float fbFireDelay;
    [SerializeField] private float fbFireDuration;
    [SerializeField] private float fbRecoveryTime;

    [SerializeField] private IIndicator[] fbAimingIndicator;
    [SerializeField] private IIndicator[] fbFiringIndicator;

    [SerializeField, Range(0, 100)] private int leadShotChance;

    private RangeAttack currLaser;

    /// <summary>
    /// Attack routine for focused attack
    /// </summary>
    /// <returns></returns>
    private IEnumerator FocusedAttack()
    {
        // Get intiial point
        Vector3 targetPos = target.Center.position;

        // flip coin for lead or not lead shot
        float chance = Random.Range(1, 100);
        bool leadShot = chance <= leadShotChance;
        Debug.Log($"Leading Shot: {leadShot}");

        Indicators.SetIndicators(fbAimingIndicator, true);

        // Aim at target, whether leading or not
        do
        {
            if(leadShot)
                targetPos = target.Center.position + (targetRB.velocity * fbFireDelay);
            else
                targetPos = target.Center.position;

            RotateToTarget(targetPos, fbRotationSpeed);
            yield return new WaitForFixedUpdate();
            yield return null;

        } while (!AcquiredTarget(targetPos, fbAccuracy));

        // Stay on target, delay but activate indicator
        Indicators.SetIndicators(fbFiringIndicator, true);
        tracker = new ScaledTimer(fbFireDelay);
        yield return new WaitUntil(tracker.TimerDone);

        

        // Fire the attack
        GameObject l = Instantiate(focusedBeamPrefab, shootPoint.position, shootPoint.rotation);
        currLaser = l.GetComponent<RangeAttack>();
        currLaser.Initialize(1, 1, fbFireDuration, gameObject);

        Indicators.SetIndicators(fbFiringIndicator, false);
        Indicators.SetIndicators(fbAimingIndicator, false);


        // Remained focused on the attack during duration
        tracker.ResetTimer(fbFireDuration);
        yield return new WaitUntil(tracker.TimerDone);

        // Recover the attack
        tracker.ResetTimer(fbRecoveryTime);
        yield return new WaitUntil(tracker.TimerDone);

        currAttack = StartCoroutine(ReturnToBase());

        yield return null;
    }

    #endregion

    #region Chase Beam Attack

    [Header("Chase Beam Attack")]

    [SerializeField] private GameObject chaseBeamPrefab;

    [SerializeField] private Transform cbStartingPosition;

    [SerializeField] private float cbRotationSpeed;

    [SerializeField] private float cbLaserDuration;

    [SerializeField] private float cbRecoveryDuration;

    [SerializeField] private float cbStartDuration;

    private IEnumerator ChaseBeamAttack()
    {
        Vector3 targetPos = cbStartingPosition.position;

        // Aim at the default
        do
        {
            RotateToTarget(targetPos, fbRotationSpeed);
            yield return new WaitForFixedUpdate();
            yield return null;

        } while (!AcquiredTarget(targetPos, fbAccuracy));

        // Fire weapon, keep looking down at the start
        GameObject l = Instantiate(chaseBeamPrefab, shootPoint.position, shootPoint.rotation);
        currLaser = l.GetComponent<RangeAttack>();
        currLaser.Initialize(1, 1, cbLaserDuration + cbStartDuration, gameObject);

        tracker = new ScaledTimer(cbStartDuration);
        yield return new WaitUntil(tracker.TimerDone);

        // Begin chasing player for duration
        tracker.ResetTimer(cbLaserDuration);

        while(!tracker.TimerDone())
        {
            targetPos = target.Center.position;
            RotateToTarget(targetPos, cbRotationSpeed);
            yield return null;
        }

        // Wait for recovery stationary
        tracker.ResetTimer(cbRecoveryDuration);
        yield return new WaitUntil(tracker.TimerDone);

        currAttack = StartCoroutine(ReturnToBase());

        yield return null;
    }

    [SerializeField] private float returnRotationSpeed;

    private IEnumerator ReturnToBase()
    {
        // Return to resting position
        while (!AcquiredTarget(restingPos, 1f))
        {
            RotateToTarget(restingPos, returnRotationSpeed);
            yield return new WaitForFixedUpdate();
            yield return null;
        }

        currAttack = null;
        yield return null;
    }

    private IEnumerator ReturnToBase(float delay)
    {
        ScaledTimer tracker = new ScaledTimer(delay);

        yield return new WaitUntil(tracker.TimerDone);

        currAttack = StartCoroutine(ReturnToBase());

        yield return null;
    }


    public void Inturrupt()
    {
        Debug.Log("Inturrupt Called!");

        if(currAttack != null)
        {
            StopCoroutine(currAttack);
            currAttack = null;

            if (currLaser != null)
                currLaser.Inturrupt();
        }

        currAttack = StartCoroutine(ReturnToBase(2f));
    }

    #endregion
}