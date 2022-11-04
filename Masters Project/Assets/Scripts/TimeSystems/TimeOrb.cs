/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - November 4th, 2022
 * Last Edited - November 4th, 2022 by Ben Schuster
 * Description - Manages the concrete behavior of a time orb
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeOrb : PickupOrb
{
    [Header("---TimeOrb Specific---")]

    [Tooltip("How much time gauge does this orb refill (in seconds)")]
    [SerializeField] private float refillAmount;

    [Tooltip("Whether the orbs can be picked up during slowed time")]
    [SerializeField] private bool pickupWhileSlowing;

    /// <summary>
    /// player time manager
    /// </summary>
    private TimeManager playerTime;

    /// <summary>
    /// collider of this object
    /// </summary>
    private SphereCollider col;

    private void Start()
    {
        playerTime = FindObjectOfType<TimeManager>();
        col = GetComponent<SphereCollider>();
        refillAmount *= 50;
        Debug.Log("Will refill " + refillAmount + " of the gauge");
    }

    /// <summary>
    /// If this object cannot be picked up during time, modify the colliders
    /// </summary>
    private void LateUpdate()
    {
        if(!pickupWhileSlowing)
        {
            if(TimeManager.WorldTimeScale < 1 && col.enabled)
            {
                col.enabled = false;
            }
            else if (TimeManager.WorldTimeScale == 1 && !col.enabled)
            {
                col.enabled = true;
            }
        }
    }

    /// <summary>
    /// Check if the time orb can be picked up
    /// </summary>
    /// <returns></returns>
    protected override bool CheckChaseRequirements()
    {
        if (!pickupWhileSlowing && TimeManager.WorldTimeScale < 1)
            return false;


        float dist = Vector3.Distance(player.position, transform.position);

        // Check if the orb can be used and if in range to chase
        return (dist <= chaseRadius) && playerTime.CurrSlowGauge < playerTime.MaxGauge();
    }

    /// <summary>
    /// What happens when this object is picked up
    /// </summary>
    protected override void OnPickup()
    {
        if(playerTime.AddGauge(refillAmount))
        {
            Destroy(gameObject);
        }
    }
}
