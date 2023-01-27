using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : Throwable
{
    [SerializeField] private GameObject explosive;

    [SerializeField] private float detonateTimer;
    [SerializeField] private bool affectedByTimestop;

    private ScaledTimer timer;

    [SerializeField] private int bounceToTimer;
    private int bounces;

    [SerializeField] private List<string> instantDetonateTags;
    

    private void Start()
    {
        if (bounceToTimer <= 0)
            StartTimer();
    }

    private void Update()
    {
        if(affectedByTimestop)
        {
            AdjustProjectileForTimestop();
        }

        if (timer != null && timer.TimerDone())
            Activate();
    }

    private void StartTimer()
    {
        timer = new ScaledTimer(detonateTimer, affectedByTimestop);
    }
    private void Activate()
    {
        Instantiate(explosive, transform.position, Quaternion.identity).GetComponent<Explosive>().Detonate();
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject rootTgt = collision.transform.root.gameObject;
        if (instantDetonateTags.Contains(rootTgt.tag))
        {
            Activate();
        }
        else
        {
            bounces++;
            if(bounces == bounceToTimer && timer is null)
            {
                StartTimer();
            }
        }
    }

    private void AdjustProjectileForTimestop()
    {
        if(TimeManager.WorldTimeScale == 1 && rb.isKinematic)
        {
            rb.isKinematic = false;
            rb.velocity = savedDir * savedMag;
        }
        else if(TimeManager.WorldTimeScale != 1 && !rb.isKinematic)
        {
            savedDir = rb.velocity.normalized;
            savedMag = rb.velocity.magnitude;

            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
        }
    }
}
