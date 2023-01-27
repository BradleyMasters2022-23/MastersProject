using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : MonoBehaviour
{
    protected Rigidbody rb;
    protected float savedMag;
    protected Vector3 savedDir;
    public void ApplyStartingForce(Vector3 force)
    {
        savedMag = force.magnitude; 
        savedDir = force.normalized;
        Debug.Log("Passed in mag of" + savedMag + " | Passed in dir of " + savedDir);
        rb = GetComponent<Rigidbody>();
        rb.AddForce(force, ForceMode.Impulse);
    }
}
