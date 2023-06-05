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

public class HealthOrb : PickupOrb
{
    [Header("---Healing Specific---")]

    [Tooltip("How much time gauge does this orb refill (in seconds)")]
    [SerializeField] private float refillAmount;

    [Tooltip("Whether the orbs can be picked up during slowed time")]
    [SerializeField] private bool pickupWhileSlowing;

    /// <summary>
    /// player time manager
    /// </summary>
    [SerializeField] private HealthManager playerHealth;

    /// <summary>
    /// collider of this object
    /// </summary>
    private SphereCollider col;

    private void Start()
    {
        col = GetComponent<SphereCollider>();
    }

    /// <summary>
    /// If this object cannot be picked up during time, modify the colliders
    /// </summary>
    private void LateUpdate()
    {
        if (playerHealth == null)
        {
            playerHealth = player.root.GetComponent<HealthManager>();
            return;
        }

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

        bool canPlayerHeal = playerHealth != null && playerHealth.CanHeal(BarType.Health);
        //Debug.Log("Can player heal: " + canPlayerHeal);
        // Check if the orb can be used and if in range to chase
        return (dist <= chaseRadius) && canPlayerHeal;
    }

    /// <summary>
    /// What happens when this object is picked up
    /// </summary>
    protected override void OnPickup()
    {
        if(playerHealth.Heal(refillAmount, BarType.Health))
        {
            OrbCollect.PlayClip(transform);
            Destroy(gameObject);
        }
    }
}
