using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;
using UnityEngine.Rendering;

public class Grenade : Throwable
{
    [SerializeField] private GameObject explosive;

    [SerializeField] private float detonateTimer;
    
    [SerializeField] private bool affectedByTimestop;

    [SerializeField] private float triggerDelay;
    [SerializeField] private float triggerRadius;
    [SerializeField] private SphereCollider triggerCol;
    private bool triggered;

    private ScaledTimer timer;

    [SerializeField] private int bounceToTimer;
    private int bounces;

    [SerializeField] private List<string> instantDetonateTags;
    [Tooltip("How much velocity is decreased by when it bounces")]
    [Range(0, 1)]
    [SerializeField] protected float velocityLossOnBounce;


    [Tooltip("Sound for launching a grenade")]
    [SerializeField] private AudioClip LaunchGrenade;
    [Tooltip("Sound when grenade explodes")]
    [SerializeField] private AudioClip ExplodeGrenade;
    private AudioSource source;

    private void Start()
    {
        if (bounceToTimer <= 0)
            StartTimer(detonateTimer);

        triggered = false;
        triggerCol.radius= triggerRadius;
        triggerCol.isTrigger= true;
        triggerCol.enabled= true;
        AudioSource.PlayClipAtPoint(LaunchGrenade, transform.position, 0.2f);
    }

    private void Update()
    {
        if (affectedByTimestop)
        {
            AdjustProjectileForTimestop();
        }

        if (timer != null && timer.TimerDone())
            Activate();
    }

    private void StartTimer(float time)
    {
        if (timer == null)
            timer = new ScaledTimer(time, affectedByTimestop);
        else
            timer.ResetTimer(time);
    }
    private void Activate()
    {
        Instantiate(explosive, transform.position, Quaternion.identity).GetComponent<Explosive>().Detonate();
        Destroy(gameObject);
        AudioSource.PlayClipAtPoint(ExplodeGrenade, transform.position, 1f);
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
        if (instantDetonateTags.Contains(rootTgt.tag))
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
            StartTimer(triggerDelay);
        }
    }

    private void Awake()
    {
        source = gameObject.AddComponent<AudioSource>();
    }
    private void AdjustProjectileForTimestop()
    {
        if (TimeManager.WorldTimeScale == 1 && rb.isKinematic)
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
