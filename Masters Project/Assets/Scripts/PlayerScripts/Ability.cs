/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - January 25, 2023
 * Last Edited - January, 2023 by Ben Schuster
 * Description - Base class for all button-based abilities with cooldowns
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public abstract class Ability : MonoBehaviour
{
    public enum AbilityStates
    {
        Ready,
        Windup,
        Recovery,
        NotReady
    }

    [Tooltip("Current state of the ability's activation")]
    private AbilityStates currentState;

    #region Ability Variables

    [Header("=== Activation Cycle Timing ===")]
    [Tooltip("Speed which the windup and recovery are modified by. % based, with 1 = 100%")]
    [SerializeField, Range(0.1f, 5)] protected float performSpeedModifier = 1;
    [Tooltip("Time it takes for the windup to triggering the ability")]
    [SerializeField] protected float windupLength;
    [Tooltip("Time it takes for the recovery after the ability triggers")]
    [SerializeField] protected float recoveryLength;
    #endregion

    #region Cooldown and Charges Variables
    [Header("=== Cooldown and charges ===")]
    [Tooltip("Time it takes to regain a charge of the ability.")]
    [SerializeField] protected float cooldown;
    [Tooltip("Speed which the cooldown is modified by. % based, with 1 = 100%")]
    [SerializeField, Range(0.1f, 5)] protected float cooldownSpeedModifier = 1;
    [Tooltip("Maximum number of charges that can be held at once")]
    [SerializeField] protected int maxCharges;
    /// <summary>
    /// current number of charges held
    /// </summary>
    [SerializeField, ReadOnly] protected int currentCharges;
    /// <summary>
    /// Timer tracker for cooldown
    /// </summary>
    protected ScaledTimer cooldownTimer;
    #endregion

    protected virtual void Start()
    {
        currentCharges = maxCharges;
    }

    #region Generic Ability Functions
    
    /// <summary>
    /// Whether this ability is ready to be used
    /// </summary>
    /// <returns></returns>
    public virtual bool IsReady()
    {
        return currentState == AbilityStates.Ready;
    }

    /// <summary>
    /// Public caller for the ability
    /// </summary>
    public void Activate()
    {
        StartCoroutine(UseAbility());
    }

    /// <summary>
    /// Called to go through the process of activating all the abilities
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator UseAbility()
    {
        // Reduce charge
        currentCharges--;

        // prepare the windup
        currentState = AbilityStates.Windup;
        yield return StartCoroutine(OnWindup());
        
        // do the actual ability. Children classes will implement concrete implementation.
        yield return StartCoroutine(OnAbility());

        // Enter recovery state
        currentState = AbilityStates.Recovery;
        yield return StartCoroutine(OnRecovery());
        

        // Based on charges amount, determine which state to return to. 
        if (currentCharges <= 0)
            currentState = AbilityStates.NotReady;
        else
            currentState = AbilityStates.Ready;

        Debug.Log("activate ability done");

        yield return null;
    }

    /// <summary>
    /// Call to activate the actual ability
    /// </summary>
    protected abstract IEnumerator OnAbility();
    /// <summary>
    /// Bonus functionality called on windup
    /// </summary>
    protected virtual IEnumerator OnWindup()
    {
        yield return new WaitForSeconds(windupLength / performSpeedModifier);
    }
    /// <summary>
    /// Bonus functionality called on recovery
    /// </summary>
    protected virtual IEnumerator OnRecovery()
    {
        yield return new WaitForSeconds(recoveryLength / performSpeedModifier);
    }

    #endregion

    #region Generic Cooldown Functions
    
    protected virtual void Update()
    {
        ChargeCooldown();
    }

    /// <summary>
    /// Manage cooldown, adding a charge if appropriate
    /// </summary>
    protected virtual void ChargeCooldown()
    {
        if (cooldownTimer is null)
        {
            cooldownTimer = new ScaledTimer(cooldown, false);
            cooldownTimer.SetModifier(cooldownSpeedModifier);
        }

        if (currentCharges < maxCharges)
        {
            if (cooldownTimer.TimerDone())
            {
                cooldownTimer.ResetTimer();
                currentCharges++;

                // check if the ability should be set ready with new charge
                if(currentState==AbilityStates.NotReady)
                    currentState= AbilityStates.Ready;
            }
        }
        else
        {
            cooldownTimer.ResetTimer();
        }
    }
    
    #endregion
}
