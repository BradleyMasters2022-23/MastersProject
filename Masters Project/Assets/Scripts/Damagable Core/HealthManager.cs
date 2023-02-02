/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - February 2nd, 2022
 * Last Edited - February 2nd, 2022 by Ben Schuster
 * Description - Control a healthpool for an entity
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HealthType
{
    Health,
    Shield,
    Armor
}

public class HealthManager : MonoBehaviour
{
    [Tooltip("The stack of healthbars this entity has. The 0 index is the most nested/final healthbar and others are damaged in that order")]
    [SerializeField] private ResourceBar[] _healthbars;
    [Tooltip("Duration of time to remain invulnerable after taking damage")]
    [SerializeField] private float _damagedInvulnerabilityDuration;
    [Tooltip("Whether or not to gate damage. Gated damage will not overflow to the healthbar below it.")]
    [SerializeField] private bool _gateHealthbarDamage;
    [Tooltip("Whether or not to gate healing. Gated healing will not overflow to the healthbar above it.")]
    [SerializeField] private bool _gateHealthbarHealing;
    /// <summary>
    /// Whether or not this entity is invulnerable
    /// </summary>
    private bool _invulnerable;

    [Tooltip("Whether or not this entity is constantly invulnerable")]
    [SerializeField] private bool _godMode;
    /// <summary>
    /// Tracker for invulnerability duration
    /// </summary>
    private ScaledTimer invulnerabilityTracker;

    /// <summary>
    /// Update to check if invulnerability is done
    /// </summary>
    private void Update()
    {
        if(_invulnerable && invulnerabilityTracker.TimerDone())
        {
            _invulnerable = false;
        }
    }

    /// <summary>
    /// Enable invulnerability for this entity for a time
    /// </summary>
    /// <param name="duration">Duration to remain in invulnerability state</param>
    public void InvulnerabilityDuration(float duration)
    {
        _invulnerable = true;

        if(invulnerabilityTracker == null) 
        {
            invulnerabilityTracker = new ScaledTimer(duration);
        }
        else
        {
            invulnerabilityTracker.ResetTimer(duration);
        }
    }

    /// <summary>
    /// Damage this entity's lowest healthbar
    /// </summary>
    /// <param name="dmg">Amount of damage to deal</param>
    /// <returns>Whether ran entity runs out of health</returns>
    public bool Damage(float dmg)
    {
        // Exit the game 
        if (_invulnerable || _godMode)
            return false;

        // activate invulnerability 
        InvulnerabilityDuration(_damagedInvulnerabilityDuration);

        float damagePool = dmg;
        
        // Apply damage onto the top most non-empty healthbar
        // Apply overflow damage if capable
        for(int i = _healthbars.Length-1; i >= 0; i--)
        {
            // Apply damage on the top bar. Do this to reset any potential regeneration timers
            damagePool = _healthbars[i].Decrease(damagePool);

            // Break if no overflow damage, or if damage was dealt and damage set to be gated
            if (damagePool == 0 || (_gateHealthbarDamage && damagePool != dmg))
            {
                break;
            }
        }

        // whether or not this entity is dead
        return _healthbars[0].IsEmptied();
    }

    /// <summary>
    /// Heal this entity's lowest healthbar
    /// </summary>
    /// <param name="hp">Amount of HP to restore</param>
    /// <param name="healthType">The type of bar this healing applies to</param>
    public void Heal(float hp, ResourceBar.BarType healthType = ResourceBar.BarType.NA)
    {
        float healPool = hp;

        for(int i = 0; i < _healthbars.Length-1; i++)
        {
            // If the bar does not fit this type, then skip it
            if (_healthbars[i].Type() != healthType && healthType != ResourceBar.BarType.NA)
                continue;

            healPool = _healthbars[i].Increase(healPool);

            // Break if no overflow healing, or if healing was applied and healing set to be gated
            if(healPool == 0 || (_gateHealthbarHealing && healPool != hp))
            {
                break;
            }
        }
    }

    /// <summary>
    /// Perform any indicator work regarding the UI
    /// </summary>
    public void UpdateIndicator()
    {
        Debug.Log("Indicator for health manager not yet implemented");
    }

    /// <summary>
    /// Enable godmode. Used for cheats or gameflow only (no idea if we doin cutscenes)
    /// </summary>
    /// <param name="enabled">whether or not to enable godmode</param>
    public void ToggleGodmode(bool enabled)
    {
        _godMode = enabled;
    }

    public bool God
    {
       get { return _godMode; }
    }
}
