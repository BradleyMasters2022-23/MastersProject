using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTrap : Trap
{
    [Header("=== Gameplay ===")]

    [Tooltip("When triggered, time it takes to begin damaging state")]
    [SerializeField] private float activationDelay;
    [Tooltip("How long to remain active in damaging state")]
    [SerializeField] private float activationTime;
    [Tooltip("How long to remain on cooldown after activation")]
    [SerializeField] private float activationCooldown;
    [Tooltip("The collider used for dealing damage")]
    [SerializeField] Collider damageCollider;

    private bool triggerable;

    [Header("=== Indicators ===")]

    [Tooltip("Any indicators that play when the trap is active and ready")]
    [SerializeField] private IIndicator[] idleIndicators;
    [Tooltip("Any indicators that play when the trap is inactive on cooldown")]
    [SerializeField] private IIndicator[] cooldownIndicators;
    [Tooltip("Any indicators that play during the activation of the trap")]
    [SerializeField] private IIndicator[] onTriggerIndicators;
    [Tooltip("Any indicators that play during the actual activation of the trap")]
    [SerializeField] private IIndicator[] activationIndicators;
    

    /// <summary>
    /// Get any references for trap and make sure core items are enabled
    /// </summary>
    private void Awake()
    {
        if(damageCollider!= null)
        {
            damageCollider.enabled = false;
            damageCollider.isTrigger= true;
            triggerable = true;
            Indicators.SetIndicators(idleIndicators, true);
        }
        else
        {
            Debug.LogError($"Groundtrap named {name} does not have a collider! Disabling trap!");
            this.enabled= false;
            return;
        }
    }

    /// <summary>
    /// Activate the floor trap if possible
    /// </summary>
    protected override void Activate()
    {
        if(triggerable)
        {
            StartCoroutine(ActivationCycle());
        }
    }

    /// <summary>
    /// Go through the entire activation cycle
    /// </summary>
    /// <returns></returns>
    private IEnumerator ActivationCycle()
    {
        Indicators.SetIndicators(idleIndicators, false);
        triggerable = false;

        // Initiate the activation delay
        Indicators.SetIndicators(onTriggerIndicators, true);
        ScaledTimer delay = new ScaledTimer(activationDelay);
        while (!delay.TimerDone())
            yield return null;
        Indicators.SetIndicators(onTriggerIndicators, false);


        // Enable the trap itself, leave it on for activation time
        Indicators.SetIndicators(activationIndicators, true);
        damageCollider.enabled = true;
        delay.ResetTimer(activationTime);
        while(!delay.TimerDone()) 
            yield return null;
        Indicators.SetIndicators(activationIndicators, false);

        // Turn off trap afterwards, manage cooldown
        Indicators.SetIndicators(cooldownIndicators, true);
        damageCollider.enabled = false;
        delay.ResetTimer(activationCooldown);
        while(!delay.TimerDone())
            yield return null;
        Indicators.SetIndicators(cooldownIndicators, false);

        triggerable = true;
        Indicators.SetIndicators(idleIndicators, true);
    }
}
