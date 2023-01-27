using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
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
}
