/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - January 20th, 2022
 * Last Edited - April 5th, 2022 by Ben Schuster
 * Description - Behavior for explosives
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Explosive : MonoBehaviour
{
    [Header("Setup")]

    [Tooltip("Physics layers to deal damage to")]
    [SerializeField] private LayerMask targetLayers;
    [Tooltip("Whether or not this should detonate instantly")]
    [SerializeField] private bool instantDetoante = false;
    
    [Tooltip("Detonate VFX reference")]
    [SerializeField] private VisualEffect VFX;
    [Tooltip("Duration to play detonate VFX for")]
    [SerializeField] private float VFXDuration = 1;
    [SerializeField, Range(0.01f, 3f)] private float effectScalingRate = 1.04f;
    /// <summary>
    /// tracker for VFX lifetime
    /// </summary>
    private ScaledTimer VFXLifeTracker;

    [Tooltip("Detonate sound reference")]
    [SerializeField] private AudioClipSO explosionSound;

    [Tooltip("Whether or not explosive is affected by timestop")]
    [SerializeField] private bool affectedByTimestop = true;
    [Tooltip("Whether or not the trigger collider turns solid in timestop")]
    [SerializeField] private bool solidInTimetop = false;

    /// <summary>
    /// reference to internal collider
    /// </summary>
    private SphereCollider col;


    [Header("Damage")]

    [Tooltip("Damage to deal to non-player targets")]
    public int damage;
    [Tooltip("Damage to deal to player targets")]
    public int playerDamage;
    [Tooltip("Radius of explosion")]
    [SerializeField] private float explosiveRadius;
    /// <summary>
    /// Targets that have been damaged and should not be targeted again
    /// </summary>
    private List<Target> damagedTargets;

    [Tooltip("Duration the damage remains effective")]
    [SerializeField] private float damageDuration = 1;
    /// <summary>
    /// Tracker for damage lifetime
    /// </summary>
    private ScaledTimer damageLifeTracker;

    [Header("Knockback")]
    [Tooltip("Horizontal knockback force to apply to targets")]
    public float horizontalForce = 200;
    [Tooltip("Vertical knockback force to apply to targets")]
    public float verticalForce = 50;
    [Tooltip("Knockback modifier graph over distance")]
    [SerializeField] private AnimationCurve knockbackFalloff;

    [Header("Utility")]
    [Tooltip("Whether or not to inturrupt enemy AI on detonate")]
    [SerializeField] private bool inturruptEnemies = false;
    [Tooltip("Whether or not to destroy bullets on detonate")]
    [SerializeField] private bool destroyBullets = false;

    /// <summary>
    /// Get all initial references. Detonate if instante detonate
    /// </summary>
    private void Awake()
    {
        // Prepare references and values
        damagedTargets = new List<Target>();
        damageLifeTracker = new ScaledTimer(damageDuration, affectedByTimestop);
        VFXLifeTracker = new ScaledTimer(VFXDuration, affectedByTimestop);
        VFX.Stop();

        // 1.08 is the general eyeball for extra increase for scale to ensure effect and radius match
        VFX.transform.localScale = Vector3.one * explosiveRadius * effectScalingRate;

        col = GetComponent<SphereCollider>();

        if (instantDetoante)
            Detonate();
    }

    /// <summary>
    /// Detonate the explosive now
    /// </summary>
    public void Detonate()
    {
        //Debug.Log("NEW GRENADE DETONATING");
        Detonate(damage);
    }
    /// <summary>
    /// Detonate the explosive now with new damage
    /// </summary>
    /// <param name="newDamage">New damage to use</param>
    public void Detonate(float newDamage)
    {
        explosionSound.PlayClip(transform);

        col.radius = explosiveRadius;
        col.enabled = true;

        Collider[] hitTargets = Physics.OverlapSphere(transform.position, explosiveRadius, targetLayers);

        VFX.Play();

        foreach (Collider target in hitTargets)
            ProcessDamage(target);
    }

    private void Update()
    {
        // If lifetimes are done, destroy explosive
        if(VFXLifeTracker.TimerDone() && damageLifeTracker.TimerDone())
        {
            Destroy(gameObject);
        }
        // If the VFX is done, disable it
        else if(VFXLifeTracker.TimerDone() && VFX.gameObject.activeInHierarchy)
        {
            VFX.Stop();
            VFX.gameObject.SetActive(false);
        }
        // If the damage is done, disable it
        else if(damageLifeTracker.TimerDone() && col.enabled)
        {
            col.enabled = false;
        }

        // if time is stopped, enable collider
        if (solidInTimetop && col.enabled)
        {
            ExplosionCol();
        }
    }


    /// <summary>
    /// Determine collider based on time scale
    /// </summary>
    private void ExplosionCol()
    {
        if(TimeManager.WorldTimeScale != 1 && col.isTrigger)
        {
            col.isTrigger = false;
        }
        else if(TimeManager.WorldTimeScale == 1 && !col.isTrigger)
        {
            col.isTrigger = true;
        }

    }

    /// <summary>
    /// Try to apply damage to the passed in collider
    /// </summary>
    /// <param name="other">object to try and damage</param>
    private void ProcessDamage(Collider other)
    {
        //Debug.Log("Checking collision with :" + other.name);
        Transform parent = other.transform;
        Target target;

        // try to destroy bullets, if possible
        // Does not work, fix later
        if (destroyBullets)
        {
            Projectile p = other.GetComponent<Projectile>();

            if (p != null)
            {
                p.Inturrupt();
                return;
            }
        }

        IDamagable damagable = other.GetComponent<IDamagable>();
        target = damagable?.Target();
        // continually escelate up for a targetable reference
        //while (!parent.TryGetComponent<Target>(out target) && parent.parent != null)
        //{
        //    parent = parent.parent;
        //}

        // if target was already damaged or not marked for team, then
        if (target != null && !damagedTargets.Contains(target))
        {
            //Debug.Log($"{target.name} not damaged, adding to list");
            damagedTargets.Add(target);

            // Make sure even if a shield is damaged, it doesn't damage the root target
            if(target.GetType() == typeof(ShieldTarget))
            {
                Target parentTarget = target.transform.root.GetComponent<Target>();
                if (parentTarget != null)
                    damagedTargets.Add(parentTarget);
            }
                
            if (target.CompareTag("Player"))
                target.RegisterEffect(playerDamage);
            else
                target.RegisterEffect(damage);

            // Apply knockback on target
            float targetDist = Vector3.Distance(transform.position, target.Center.position);
            float knockback = knockbackFalloff.Evaluate(targetDist);
            target.Knockback(
                horizontalForce * knockback,
                verticalForce * knockback,
                transform.position);

            if (inturruptEnemies)
                target.Inturrupt();
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    //Debug.Log("Checking collision with :" + other.name);
    //    Transform parent = other.transform;
    //    Target target;

    //    // try to destroy bullets, if possible
    //    // Does not work, fix later
    //    if (destroyBullets)
    //    {
    //        Projectile p = other.GetComponent<Projectile>();

    //        if(p != null) 
    //        {
    //            p.Inturrupt();
    //            return;
    //        }
    //    }

    //    // continually escelate up for a targetable reference
    //    while (!parent.TryGetComponent<Target>(out target) && parent.parent != null)
    //    {
    //        parent = parent.parent;
    //    }
    //    //Debug.Log("parent landed on " + parent.name);


    //    // if target was already damaged or not marked for team, then
    //    if (target != null && damagableTags.Contains(target.tag) && !damagedTargets.Contains(target))
    //    {
    //        //Debug.Log($"{target.name} not damaged, adding to list");
    //        damagedTargets.Add(target);
    //        damagedTargets.Add(target);

    //        if (target.CompareTag("Player"))
    //            target.RegisterEffect(playerDamage);
    //        else
    //            target.RegisterEffect(damage);

    //        // Apply knockback on target
    //        float targetDist = Vector3.Distance(transform.position, target.Center.position);
    //        float knockback = knockbackFalloff.Evaluate(targetDist);
    //        target.Knockback(
    //            horizontalForce * knockback, 
    //            verticalForce * knockback, 
    //            transform.position);

    //        if (inturruptEnemies)
    //            target.Inturrupt();
    //    }
}
