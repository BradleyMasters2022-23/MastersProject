/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - January 30th, 2023
 * Last Edited - January 30th, 2023 by Ben Schuster
 * Description - Manages an exploding greande
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : Throwable, TimeObserver
{
    [SerializeField] private GameObject explosive;

    [SerializeField] private float detonateTimer;
    
    [SerializeField] private float triggerDelay;
    [SerializeField] private float triggerRadius;
    [SerializeField] private SphereCollider triggerCol;
    private bool triggered;

    private LocalTimer timer;

    [SerializeField] private int bounceToTimer;
    private int bounces;

    [SerializeField] private List<string> instantDetonateTags;
    [Tooltip("How much velocity is decreased by when it bounces")]
    [Range(0, 1)]
    [SerializeField] protected float velocityLossOnBounce;


    [Tooltip("Sound of grenade flying through the air")]
    [SerializeField] private AudioClipSO flyGrenade;
    [Tooltip("Sound when grenade is triggered and about to explode")]
    [SerializeField] private AudioClipSO triggerGrenade;

    private AudioSource source;

    private void Start()
    {
        if (bounceToTimer <= 0)
            StartTimer(detonateTimer);

        triggered = false;
        triggerCol.radius= triggerRadius;
        triggerCol.isTrigger= true;
        triggerCol.enabled= true;

        flyGrenade.PlayClip(source);
    }

    private void Update()
    {

        if (timer != null && timer.TimerDone())
            Activate();
    }

    private void StartTimer(float time)
    {
        if (timer == null)
            timer = GetTimer(time);
        else
            timer.ResetTimer(time);
    }
    private void Activate()
    {
        Instantiate(explosive, transform.position, Quaternion.identity).GetComponent<Explosive>()?.Detonate();
        Destroy(gameObject);
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(velocityLossOnBounce > 0)
        {
            Vector3 temp = rb.velocity;
            temp.x *= (1 - velocityLossOnBounce);
            temp.z *= (1 - velocityLossOnBounce);
            rb.velocity = temp;
        }
            

        GameObject rootTgt = collision.transform.root.gameObject;
        if (instantDetonateTags.Contains(rootTgt.tag) && !triggered)
        {
            Activate();
        }
        else
        {
            bounces++;
            if(bounces == bounceToTimer && timer is null)
            {
                StartTimer(detonateTimer);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject rootTgt = other.transform.root.gameObject;
        if (instantDetonateTags.Contains(rootTgt.tag) && !triggered)
        {
            triggered = true;
            triggerGrenade.PlayClip(transform);
            StartTimer(triggerDelay);
        }
    }

    private void Awake()
    {
        source = gameObject.AddComponent<AudioSource>();
    }

    public void OnStop()
    {
        savedDir = rb.velocity.normalized;
        savedMag = rb.velocity.magnitude;

        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
    }

    public void OnResume()
    {
        rb.isKinematic = false;
        rb.velocity = savedDir * savedMag;
    }
}
