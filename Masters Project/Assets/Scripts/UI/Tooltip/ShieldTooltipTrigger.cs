/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - April 4th, 2022
 * Last Edited - April 4th, 2022 by Ben Schuster
 * Description - Manages tooltip behavior for shields
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldTooltipTrigger : TooltipHolder
{
    /// <summary>
    /// Public static reference to an instance
    /// </summary>
    public static ShieldTooltipTrigger instance;

    [Tooltip("Amount of times to shoot a shield without destroying it to display the tooltip")]
    [SerializeField] private int triggerHitCount = 20;
    /// <summary>
    /// current count of hits without destroying shield
    /// </summary>
    private int hitCount = 0;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    /// <summary>
    /// Increment tooltip requirement and determine if it should display
    /// </summary>
    public void ShieldHit()
    {
        hitCount++;

        if (hitCount == triggerHitCount)
        {
            SubmitTooltip();
        }
    }

    /// <summary>
    /// If the shield was killed, retract a tooltip and reset counter
    /// </summary>
    public void ShieldKilled()
    {
        RetractTooltip();
        hitCount = 0;
    }
}
