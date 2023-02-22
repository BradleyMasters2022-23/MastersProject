/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - February 17th, 2022
 * Last Edited - February 17th, 2022 by Ben Schuster
 * Description - Abstract base for traps, handling trigger registering
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Trap : MonoBehaviour, ITriggerable
{
    [Header("=== Triggers ===")]

    [Tooltip("Any triggers that activate this trap")]
    [SerializeField] private Trigger[] activationTriggers;

    /// <summary>
    /// Register this trap to any set triggers.
    /// Do in start so other triggers have time to enable themselves in Awake
    /// </summary>
    private void Start()
    {
        foreach(Trigger trigger in activationTriggers)
        {
            trigger.Register(this);
        }
    }

    /// <summary>
    /// Just to be safe, unsubscribe from triggers 
    /// </summary>
    private void OnDisable()
    {
        foreach (Trigger trigger in activationTriggers)
        {
            trigger.Unregister(this);
        }
    }

    /// <summary>
    /// Activate the trap, whatever it is
    /// </summary>
    protected abstract void Activate();

    /// <summary>
    /// Public caller for the trigger
    /// </summary>
    public void Trigger()
    {
        Activate();
    }
}
