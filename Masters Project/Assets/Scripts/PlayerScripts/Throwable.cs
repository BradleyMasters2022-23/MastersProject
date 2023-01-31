using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : MonoBehaviour
{
    protected Rigidbody rb;
    protected float savedMag;
    protected Vector3 savedDir;

    protected bool _isGhost;
    public void ApplyStartingForce(Vector3 force, bool isGhost = false)
    {
        _isGhost = isGhost;
        savedMag = force.magnitude; 
        savedDir = force.normalized;
        rb = GetComponent<Rigidbody>();

        if(isGhost)
        {
            rb.isKinematic = false;
        }
        StartCoroutine(WaitForFlight(force));
    }

    protected IEnumerator WaitForFlight(Vector3 force)
    {
        while(TimeManager.WorldTimeScale != 1)
        {
            yield return null;
        }
        rb.AddForce(force, ForceMode.Impulse);
    }
}
