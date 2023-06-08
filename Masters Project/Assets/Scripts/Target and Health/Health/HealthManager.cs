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
using UnityEngine.Events;

public enum HealthType
{
    Health,
    Shield,
    Armor
}

public class HealthManager : MonoBehaviour
{
    [Tooltip("The healthbars to load for this entity. Index 0 is the last required healthbar")]
    [SerializeField] private ResourceBarSO[] _healthbarData;
    /// <summary>
    /// Direct references to the required resource bar modules
    /// </summary>
    private ResourceBar[] _healthbars;

    private bool initialized = false;
    public bool Initialized
    {
        get { return initialized; }
    }

    [Tooltip("Duration of time to remain invulnerable after taking damage")]
    [SerializeField] private float _damagedInvulnerabilityDuration;
    [Tooltip("Whether or not to gate damage. Gated damage will not overflow to the healthbar below it.")]
    [SerializeField] private bool _gateHealthbarDamage;
    [Tooltip("Whether or not to gate healing. Gated healing will not overflow to the healthbar above it.")]
    [SerializeField] private bool _gateHealthbarHealing;
    [Tooltip("When health is gated, how long to remain invulnerable for afterwards")]
    [SerializeField] private float _gatedHealthInvulnerability;

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

    public delegate void OnBarTransition();

    public OnBarTransition onHealthbarLostEvents;

    public UnityAction onDamagedEvent;

    public float timeScale;

    public bool Init()
    {
        if(_healthbarData.Length <= 0)
        {
            Debug.LogError($"Entity {gameObject.name} has a health manager, but no healthbar data!" +
                            $"Did you forget to assign its data? \nDestroying entity this run.");
            return false;
        }

        _healthbars = new ResourceBar[_healthbarData.Length];
        for(int i = 0; i < _healthbarData.Length; i++)
        {
            _healthbars[i] = new ResourceBar();
            _healthbars[i].Init(_healthbarData[i]);
        }

        timeScale = 1;
        initialized = true;
        return true;
    }

    /// <summary>
    /// Update to check if invulnerability is done
    /// </summary>
    private void Update()
    {
        if (!initialized)
            return;


        if(_invulnerable && invulnerabilityTracker.TimerDone())
        {
            _invulnerable = false;
        }
    }

    private void FixedUpdate()
    {
        if(!initialized) return;

        // check if any of the resource bars should regenerate
        foreach(ResourceBar bar in _healthbars)
        {
            bar.SetTimesale(timeScale);
            bar.StateUpdateFunction();
        }
    }

    /// <summary>
    /// Enable invulnerability for this entity for a time
    /// </summary>
    /// <param name="duration">Duration to remain in invulnerability state</param>
    public void InvulnerabilityDuration(float duration)
    {
        if(duration<= 0)
        {
            return;
        }

        _invulnerable = true;

        if(invulnerabilityTracker == null) 
        {
            invulnerabilityTracker = new ScaledTimer(duration, false);
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

        onDamagedEvent?.Invoke();

        // activate invulnerability 
        InvulnerabilityDuration(_damagedInvulnerabilityDuration);

        float damagePool = dmg;
        int lowestHealthbarAffected = 0;

        // Apply damage onto the top most non-empty healthbar
        // Apply overflow damage if capable
        for(int i = _healthbars.Length-1; i >= 0; i--)
        {
            // Apply damage on the top bar. Do this to reset any potential regeneration timers
            damagePool = _healthbars[i].Decrease(damagePool);
            lowestHealthbarAffected = i;

            // If the healthbar was lost and damage dealt, perform any on lost events
            if(damagePool < dmg && _healthbars[i].IsEmptied())
            {
                onHealthbarLostEvents?.Invoke();

                if (_gateHealthbarDamage)
                {
                    InvulnerabilityDuration(_gatedHealthInvulnerability);
                    break;
                }
            }

            // Break if no more damage left to deal
            if (damagePool == 0)
            {
                break;
            }
        }

        // whether or not this entity is dead
        return _healthbars[0].IsEmptied();
    }

    /// <summary>
    /// Determine if this target can be healed based on the corresponding health type
    /// </summary>
    /// <param name="healthType">Type of healthbar trying to be healed</param>
    /// <returns>Whether the target can heal</returns>
    public bool CanHeal(BarType healthType)
    {
        // Check each healthbar of this type, see if any of them are empty
        for (int i = 0; i < _healthbars.Length; i++)
        {
            //Debug.Log($"Comparing healthbar index {i} | Type : {_healthbars[i].Type()} | Can heal : {!_healthbars[i].IsFull()}");
            if (_healthbars[i].Type() == healthType && !BufferFull())
            {
                return true;
            }
        }
        return false;
    }

    #region Healing Buffer

    /// <summary>
    /// Current buffer of incoming health
    /// </summary>
    private List<HealthOrb> healingBuffer = new List<HealthOrb>();

    /// <summary>
    /// Add an orb to potential healing buffer
    /// </summary>
    /// <param name="orb">Orb to add to buffer</param>
    public void AddToBuffer(HealthOrb orb)
    {
        if (!healingBuffer.Contains(orb))
        {
            healingBuffer.Add(orb);
        }
            
    }
    /// <summary>
    /// Remove orb from potential healing buffer
    /// </summary>
    /// <param name="orb">Orb to remove from buffer</param>
    public void RemoveFromBuffer(HealthOrb orb)
    {
        if (healingBuffer.Contains(orb))
        {
            healingBuffer.Remove(orb);
        }
            
    }
    /// <summary>
    /// Check if the healing potential is above the necessary to fully heal
    /// </summary>
    private bool BufferFull()
    {
        float healingPot = 0;
        foreach(HealthOrb o in healingBuffer.ToArray())
        {
            healingPot += o.GetAmt();
        }

        return (CurrentHealth(0) + healingPot) >= MaxHealth(0);
    }

    #endregion

    /// <summary>
    /// Heal this entity's lowest healthbar
    /// </summary>
    /// <param name="hp">Amount of HP to restore</param>
    /// <param name="healthType">The type of bar this healing applies to</param>
    /// <return>Whether any healing was done</return>
    public bool Heal(float hp, BarType healthType = BarType.NA)
    {
        float healPool = hp;

        if(_healthbars == null)
        {
            Debug.Log("ENEMY HEATH BUG DETECTED");
            return false;
        }

        for(int i = 0; i < _healthbars.Length; i++)
        {
            // If the bar does not fit this type, then skip it
            if (_healthbars[i].Type() != healthType && healthType != BarType.NA)
                continue;

            healPool = _healthbars[i].Increase(healPool);

            // Break if no overflow healing, or if healing was applied and healing set to be gated
            if(healPool == 0 || (_gateHealthbarHealing && healPool != hp))
            {
                break;
            }
        }

        return hp != healPool;
    }

    /// <summary>
    /// Reset the health of this manager
    /// </summary>
    public void ResetHealth()
    {
        // turn off gated healing for the full heal
        bool originalSetting = _gateHealthbarHealing;
        _gateHealthbarHealing = false;

        Heal(9999);

        // set it back to its original setting
        _gateHealthbarHealing = originalSetting;
    }

    /// <summary>
    /// Perform any indicator work regarding the UI
    /// </summary>
    public void UpdateIndicator()
    {
        Debug.Log("Indicator for health manager not yet implemented");
    }

    #region Getters

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

    public ResourceBar ResourceBarAtIndex(int index)
    {
        return _healthbars[index];
    }
    public ResourceBarSO ResourceDataAtIndex(int index)
    {
        return _healthbarData[index];
    }

    /// <summary>
    /// Get the current value of the health, given the index
    /// </summary>
    /// <param name="index">index healthbar to use</param>
    /// <returns>current health of that healthbar</returns>
    public float CurrentHealth(int index)
    {
        if (index >= _healthbars.Length)
            return 0;

        return _healthbars[index].CurrentValue();
    }

    /// <summary>
    /// Get the max health of the indexed healthbar
    /// </summary>
    /// <param name="index">index healthbar to use</param>
    /// <returns>current health of that max </returns>
    public float MaxHealth(int index)
    {
        if (index >= _healthbars.Length)
            return 0;

        return _healthbars[index].MaxValue();
    }

    #endregion

    #region Upgrade Functions

    public void IncreaseMaxHealth(float increment, int healthbarIndex = 0)
    {
        _healthbars[healthbarIndex].IncreaseMax(increment, true);
    }

    public void DecreaseMaxHealth(float decrement, int healthbarIndex = 0)
    {
        _healthbars[healthbarIndex].DecreaseMax(decrement);

    }

    #endregion
}
