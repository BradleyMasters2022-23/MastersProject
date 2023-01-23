using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class FlightAccelerating : MonoBehaviour
{

    [SerializeField] private float startSpeed;
    [SerializeField] private bool affectedByTimestop;
    [SerializeField] private float acceleratingRate;
    [SerializeField] private float targetEndVelocity;


    private Rigidbody rb;

    private Vector3 targetDir;
    private float targetVel;
    private float flyTime;

    private bool reachedTarget;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        reachedTarget = false;

        targetVel = startSpeed;
        targetDir = transform.forward;
        flyTime = 1;
    }
    private void FixedUpdate()
    {
        if (affectedByTimestop)
        {
            if(!reachedTarget)
            {
                flyTime += TimeManager.WorldDeltaTime;
                targetVel = startSpeed * Mathf.Pow(acceleratingRate, flyTime);
            }
            else if(targetVel != targetEndVelocity)
            {
                targetVel = targetEndVelocity;
            }
            
            rb.velocity = targetVel * targetDir * TimeManager.WorldTimeScale;
        }
        else
        {
            if(!reachedTarget)
            {
                flyTime += Time.deltaTime;
                targetVel = startSpeed * Mathf.Pow(acceleratingRate, flyTime);
            }
            else if (targetVel != targetEndVelocity)
            {
                targetVel = targetEndVelocity;
            }
            
            rb.velocity = targetVel * targetDir;
        }

        // if accelerating, check to see if it reached its max velocity
        if (!reachedTarget 
            && ((acceleratingRate > 1 && targetVel >= targetEndVelocity)
            || (acceleratingRate < 1 && targetVel <= targetEndVelocity)))
        {
            reachedTarget = true;
        }
    }
}
