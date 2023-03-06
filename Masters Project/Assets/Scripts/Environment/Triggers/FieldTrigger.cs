/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - February 17th, 2022
 * Last Edited - February 17th, 2022 by Ben Schuster
 * Description - Concrete trigger integrated with a collider trigger
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Collider))]
public class FieldTrigger : Trigger
{
    [Tooltip("All potential tags that can trigger this collider")]
    [SerializeField] private List<string> triggerableTags;

    [Tooltip("Whether this trigger can only be used once")]
    [SerializeField] private bool singleUse;
    [Tooltip("Whether this trigger has a cooldown. Set to 0 for no cooldown. ")]
    [HideIf("@this.singleUse == true")]
    [SerializeField] private float activationCooldown;

    /// <summary>
    /// The cooldown tracker
    /// </summary>
    private ScaledTimer cooldown;
    /// <summary>
    /// Refernece to this triggers collider
    /// </summary>
    private Collider[] col;

    protected override void Awake()
    {
        base.Awake();

        // Get colliders and set to trigger
        col = GetComponents<Collider>();
        foreach(Collider c in col)
            c.isTrigger = true;

        // if not single use and there is a cooldown, create its timer
        if (!singleUse && activationCooldown > 0)
        {
            cooldown = new ScaledTimer(activationCooldown, true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if theres any trigger to activate, otherwise 
        // disable this script since its useless
        if (subscribers == null)
        {
            this.enabled = false;
            return;
        }
        else if(subscribers.Count <= 0)
        {
            return;
        }


        //Debug.Log($"Checking collision with {other.name}");

        // Do not activate if a cooldown timer is active
        if (cooldown != null && !cooldown.TimerDone())
        {
            //.Log($"Trigger still on cooldown. Progress {cooldown.TimerProgress()}");
            return;
        }

        // Check if collider is in tag
        if (triggerableTags.Contains(other.transform.root.tag))
        {
            Activate();

            // if single use, turn off collider
            if (singleUse)
            {
                foreach (Collider c in col)
                    c.enabled = false;
            }
            // otherwise if theres a cooldown, reset it
            else if(cooldown != null)
            {
                cooldown.ResetTimer();
            }
        }
    }
}
