/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - November 4th, 2022
 * Last Edited - July 31st, 2023 by Ben Schuster
 * Description - Manages the concrete behavior of a time orb
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthOrb : PickupOrb, IPoolable
{
    [Header("---Healing Specific---")]

    [Tooltip("How much time gauge does this orb refill (in seconds)")]
    [SerializeField] private float refillAmount;

    /// <summary>
    /// player time manager
    /// </summary>
    private PlayerHealthManager playerHealth;

    /// <summary>
    /// If this object cannot be picked up during time, modify the colliders
    /// </summary>
    protected override void LateUpdate()
    {
        if (playerHealth == null)
        {
            playerHealth = player.root.GetComponent<PlayerHealthManager>();
            return;
        }

        base.LateUpdate();
    }

    /// <summary>
    /// Check if the time orb can be picked up
    /// </summary>
    /// <returns></returns>
    protected override bool CheckChaseRequirements()
    {
        if ((!pickupWhileSlowing && Slowed) || !InRange())
            return false;

        bool canHeal = playerHealth != null && playerHealth.CanHeal(BarType.Health);

        if (canHeal)
        {
            playerHealth.AddToBuffer(this);
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
        playerHealth.RemoveFromBuffer(this);
        playerHealth.Heal(refillAmount, BarType.Health);
    }

    public float GetAmt()
    {
        return refillAmount;
    }

    public override void PoolPush()
    {
        base.PoolPush();
        playerHealth?.RemoveFromBuffer(this);
    }
}
