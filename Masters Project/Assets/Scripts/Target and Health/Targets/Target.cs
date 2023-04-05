/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - February 2nd, 2022
 * Last Edited - February 2nd, 2022 by Ben Schuster
 * Description - Base class for ALL targetable entities
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public enum Team
{
    UNASSIGNED,
    PLAYER,
    ENEMY,
    WORLD
}


[System.Serializable]
public class DroppableQuantity
{
    [AssetsOnly] public GameObject spawnObject;
    public Vector2 quantityRange;
    [Range(0, 100)] public float dropChance;
}

[RequireComponent(typeof(HealthManager))]
public abstract class Target : MonoBehaviour
{
    [Header("Core Target Info")]

    [Tooltip("The center point of this entity, used for targeting and spawning")]
    [SerializeField] protected Transform _center;
    [Tooltip("The team this target is a part of")]
    [SerializeField] protected Team _team = Team.UNASSIGNED;
    [Tooltip("The threat of this target, used for AI targeting")]
    [SerializeField] protected float _targetThreat = 0;
    [Tooltip("Whether or not this target is killable. Unkillable targets still take damage but cannot die.")]
    [SerializeField] protected bool _unkillable;
    /// <summary>
    /// Rigidbody for this target, if it exists
    /// </summary>
    protected Rigidbody _rb;

    /// <summary>
    /// The manager controlling health for this target
    /// </summary>
    protected HealthManager _healthManager;

    /// <summary>
    /// Whether or not this entity has already been killed
    /// </summary>
    protected bool _killed = false;

    [Header("Core Gameplay Features")]

    [Tooltip("Whether or not this target is immune to knockback")]
    [SerializeField] protected bool immuneToKnockback = true;
    [SerializeField, ShowIf("@this.immuneToKnockback == false")] protected float maxKnockback;
    [Tooltip("If enabled, this target can not take damage until the shield is destroyed")]
    [SerializeField] protected ShieldTarget invincibilityShield;
    [SerializeField] protected float shieldDeathImmunity;

    // The manger controlling buffs and debuffs for this target
    //private EffectManager _effectManager;

    [Header("Core Visual Info")]

    [SerializeField, AssetsOnly] protected GameObject _deathVFX;

    [Tooltip("Damage made when this target is damaged")]
    [SerializeField] protected AudioClipSO damagedSound;
    [Tooltip("Damage made when this target is killed")]
    [SerializeField] protected AudioClipSO deathSound;

    [Tooltip("Cooldown between ability for damaged sound effect play")]
    [SerializeField] private float damagedSoundCooldown = 0.5f;

    private ScaledTimer damagedSoundCooldownTracker;

    [Header("Drop Stuff")]
    [SerializeField] protected List<DroppableQuantity> dropList;

    /// <summary>
    /// Audiosource for this target
    /// </summary>
    protected AudioSource audioSource;

    protected virtual void Awake()
    {
        _healthManager = GetComponent<HealthManager>();
        _rb = GetComponent<Rigidbody>();
        damagedSoundCooldownTracker = new ScaledTimer(damagedSoundCooldown, false);
        // If initialization of health manager fails, destroy itself
        if (_healthManager == null || !_healthManager.Init())
        {
            KillTarget();
        }
    }

    // Placeholder, needs to take in 'TargetableEffect' figure out whether to send this data to the health manager or effect manager
    /// <summary>
    /// Register an effect against this target.
    /// </summary>
    /// <param name="dmg">PLACEHOLDER - pass damage to deal to this target</param>
    public virtual void RegisterEffect(float dmg)
    {
        if(!AffectedByAttacks()) return;

        if(damagedSoundCooldownTracker != null && damagedSoundCooldownTracker.TimerDone())
        {
            damagedSound.PlayClip(_center, audioSource);
            damagedSoundCooldownTracker.ResetTimer();
        }
        

        if (!_killed && _healthManager.Damage(dmg))
        {
            if (_unkillable)
                return;

            _killed = true;
            KillTarget();
        }
            
    }

    /// <summary>
    /// Whether or not this target can be affected by attacks
    /// </summary>
    /// <returns></returns>
    protected bool AffectedByAttacks()
    {
        // if a shield is enabled, dont take damage
        if (invincibilityShield != null && invincibilityShield.isActiveAndEnabled)
            return false;

        return !_killed;
    }

    /// <summary>
    /// Kill this target. Default with little functionality
    /// </summary>
    protected virtual void KillTarget()
    {
        DeathEffects();

        DropAllObjs();

        // Debug.Log($"Entity {gameObject.name} has been killed but does not have its own kill function, destroying self.");
        DestroyObject();
    }

    protected virtual void DestroyObject()
    {
        Destroy(gameObject);
    }

    public void DropAllObjs()
    {
        foreach (DroppableQuantity obj in dropList)
        {
            SpawnDrops(obj.spawnObject, obj.dropChance, obj.quantityRange);
        }
    }

    public void DeathEffects()
    {
        deathSound.PlayClip(_center);

        if (_deathVFX != null)
            Instantiate(_deathVFX, _center.position, Quaternion.identity);
    }

    /// <summary>
    /// Inturrupt the target. By default, do nothing.
    /// </summary>
    public virtual void Inturrupt()
    {
        return;
    }

    /// <summary>
    /// Reset the core values of the target
    /// </summary>
    public virtual void ResetTarget()
    {
        _killed = false;
        _healthManager.ResetHealth();

        if (invincibilityShield != null)
            invincibilityShield.ResetTarget();
    }
    
    //public void ShieldDestroyed()
    //{
    //    _healthManager.InvulnerabilityDuration(shieldDeathImmunity);
    //}

    /// <summary>
    /// Determine if drops should drop, and spawn them
    /// </summary>
    protected void SpawnDrops(GameObject orb, float dropChance, Vector2 dropAmountRange)
    {
        // Roll the dice. If the value is higher than the value, exit
        if (Random.Range(1f, 100f) > dropChance || orb is null)
        {
            return;
        }

        int spawnAmount = Random.Range((int)dropAmountRange.x, (int)dropAmountRange.y);

        // Spawn the randomized amount at random ranges, as long as they aren't intersecting
        for (int i = 0; i < spawnAmount; i++)
        {
            // Spawn objects, apply rotation and velocity
            if (transform != null)
                Instantiate(orb, _center.position, Quaternion.identity);
        }
    }

    public virtual void Knockback(float force, float verticalForce, Vector3 origin)
    {
        if (!AffectedByAttacks())
            return;

        // make sure knockback can be applied
        if (immuneToKnockback || _rb == null || _rb.isKinematic)
            return;

        // Calculate force vector, draw for debug reasons
        Vector3 forceVector = (_center.position - origin).normalized * force;
        forceVector += Vector3.up * verticalForce;
        // Clamp max knockback
        if(forceVector.magnitude > maxKnockback)
            forceVector= forceVector.normalized * maxKnockback;
        Debug.DrawRay(origin, forceVector, Color.red, 3f);

        // Zero current velocity, apply new force
        _rb.velocity= Vector3.zero;
        _rb.AddForce(forceVector, ForceMode.Impulse);
    }

    #region Getters

    /// <summary>
    /// The center point of this entity, used for targeting and spawning
    /// </summary>
    public Transform Center
    {
        get { return _center; }
    }
    /// <summary>
    /// The team this target is a part of
    /// </summary>
    public Team Team
    {
        get { return _team; }
    }
    /// <summary>
    /// The threat of this target, used for AI targeting
    /// </summary>
    public float Threat
    {
        get { return _targetThreat; }
    }

    /// <summary>
    /// Whether or not this target is killed
    /// </summary>
    /// <returns></returns>
    public bool Killed()
    {
        return _killed;
    }

    #endregion
}
