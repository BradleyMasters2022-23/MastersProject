/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - June 13th, 2023
 * Last Edited - June 13th, 2023 by Ben Schuster
 * Description - Move the current object from one position to another
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MoveTo : MonoBehaviour
{
    [SerializeField] float startDelay;
    [SerializeField] Transform startingTarget;
    [SerializeField] float endDelay;
    [SerializeField] Transform endingTarget;

    [SerializeField] private float maxSpeed;
    [SerializeField] private AnimationCurve accelerationRate;

    [SerializeField] private UnityEvent onStartEvents;
    [SerializeField] private UnityEvent onEndEvents;

    /// <summary>
    /// Perform the prepared movement. Automatically enables the object. 
    /// </summary>
    public void Move()
    {
        gameObject.SetActive(true);
        transform.position = startingTarget.position;
        StartCoroutine(MoveToRoutine());
    }

    private IEnumerator MoveToRoutine()
    {
        // Get main dist
        float maxDist = Vector3.Distance(startingTarget.position, endingTarget.position);
        float currDist = maxDist;
        float distRatio;

        // perform start events after a delay
        yield return new WaitForSeconds(startDelay);
        onStartEvents.Invoke();

        ScaledTimer temp = new ScaledTimer(10, false);

        // Continually go towards target. Currently just a straight shot
        while(currDist >= 0.1f)
        {
            // Get current dist ratio for the acceleration curve
            currDist = Vector3.Distance(transform.position, endingTarget.position);
            distRatio = 1 - (currDist / maxDist);

            // if close enough or reachged max time, go to end
            if (currDist <= maxDist || temp.TimerDone())
            {
                transform.position = endingTarget.position;
                break;
            }
            // Move towards the end target, bottled by max speed and accounting for its current dist ratio
            transform.Translate(
                (endingTarget.position - transform.position).normalized * 
                (maxSpeed * accelerationRate.Evaluate(distRatio)));

            yield return new WaitForFixedUpdate();
        }

        // perform end events after a delay
        yield return new WaitForSeconds(endDelay);
        onEndEvents.Invoke();
    }
}
