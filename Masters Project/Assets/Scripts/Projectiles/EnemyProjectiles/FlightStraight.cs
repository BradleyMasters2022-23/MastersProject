using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightStraight : MonoBehaviour
{

    [SerializeField] private float startSpeed;
    [SerializeField] private bool affectedByTimestop;

    private Rigidbody rb;

    private Vector3 targetDir;
    private float targetVel;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        targetVel = startSpeed;
        targetDir = transform.forward;




    }

    private void FixedUpdate()
    {
        if(affectedByTimestop)
        {
            rb.velocity = targetVel * targetDir * TimeManager.WorldTimeScale;
        }
        else
        {
            rb.velocity = targetVel * targetDir;
        }
    }
}
