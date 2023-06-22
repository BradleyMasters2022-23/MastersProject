/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - February 3rd, 2022
 * Last Edited - February 3rd, 2022 by Ben Schuster
 * Description - Manages the concrete behavior of a health orb
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeOrb : PickupOrb, IPoolable
{
    [Header("---TimeOrb Specific---")]

    [Tooltip("How much time gauge does this orb refill (in seconds)")]
    [SerializeField] private float refillAmount;

    /// <summary>
    /// player time manager
    /// </summary>
    private TimeManager playerTime;

    private void Start()
    {
        playerTime = FindObjectOfType<TimeManager>();
        refillAmount *= 50;
    }

    /// <summary>
    /// Check if the time orb can be picked up
    /// </summary>
    /// <returns></returns>
    protected override bool CheckChaseRequirements()
    {
        if ((!pickupWhileSlowing && Slowed) || !InRange())
            return false;

        bool canHeal = !playerTime.IsFull();

        if (canHeal)
        {
            playerTime.AddToBuffer(this);
            return true;
        }
        else
            return false;
    }

    /// <summary>
    /// What happens when this object is picked up
    /// </summary>
    protected override void OnPickup()
    {
        // Try adding to the gauge. False means it was full and not picked up 
        playerTime.RemoveFromBuffer(this);
        playerTime.AddGauge(refillAmount);
    }

    public float GetAmt()
    {
        return refillAmount;
    }

    public override void PoolPush()
    {
        base.PoolPush();
        playerTime?.RemoveFromBuffer(this);
    }
}
