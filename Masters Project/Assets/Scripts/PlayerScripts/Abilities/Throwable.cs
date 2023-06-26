/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - January 30th, 2022
 * Last Edited - January 30th, 2022 by Ben Schuster
 * Description - Main class for any throwable object such as a grenade
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : TimeAffectedEntity
{
    protected Rigidbody rb;
    protected float savedMag;
    protected Vector3 savedDir;

    public void ApplyStartingForce(Vector3 force, bool isGhost = false)
    {
        savedMag = force.magnitude; 
        savedDir = force.normalized;
        rb = GetComponent<Rigidbody>();

        StartCoroutine(WaitForFlight(force));
    }

    protected IEnumerator WaitForFlight(Vector3 force)
    {
        while(TimeManager.TimeStopped)
        {
            yield return null;
        }
        rb.AddForce(force, ForceMode.Impulse);
    }
}
