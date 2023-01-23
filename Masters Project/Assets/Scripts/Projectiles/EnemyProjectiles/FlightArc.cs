using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class FlightArc : MonoBehaviour
{
    [SerializeField] private float startSpeed;
    [SerializeField] private bool affectedByTimestop;
    [SerializeField] private float startAngle;

    private Rigidbody rb;

    private Vector3 targetDir;
    private float targetVel;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;

        transform.Rotate(Vector3.right, -startAngle);

        targetVel = startSpeed;
        targetDir = transform.forward;
        //targetDir.x += startAngle;

        Debug.DrawRay(transform.position, transform.forward, Color.red, 0.5f);

        rb.AddForce(transform.forward * startSpeed, ForceMode.Impulse);
    }

    private void FixedUpdate()
    {
        if (affectedByTimestop)
        {
            //rb.velocity = targetVel * targetDir * TimeManager.WorldTimeScale;
        }
        else
        {
            //rb.velocity = targetVel * targetDir;
        }
    }
}
