using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : MonoBehaviour
{
    protected Rigidbody rb;
    protected float savedMag;
    protected Vector3 savedDir;
    [SerializeField] protected float timestopFreezeThreshold;



    public void ApplyStartingForce(Vector3 force, bool isGhost = false)
    {
        savedMag = force.magnitude; 
        savedDir = force.normalized;
        rb = GetComponent<Rigidbody>();

        StartCoroutine(WaitForFlight(force));
    }

    protected IEnumerator WaitForFlight(Vector3 force)
    {
        while(TimeManager.WorldTimeScale <= timestopFreezeThreshold)
        {
            yield return null;
        }
        rb.AddForce(force, ForceMode.Impulse);
    }
}
