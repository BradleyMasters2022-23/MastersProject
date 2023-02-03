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
    /// The manager controlling health for this target
    /// </summary>
    protected HealthManager _healthManager;

    // The manger controlling buffs and debuffs for this target
    //private EffectManager _effectManager;

    /// <summary>
    /// Whether or not this entity has already been killed
    /// </summary>
    protected bool _killed = false;

    [Header("Core Visual Info")]

    [SerializeField, AssetsOnly] private GameObject _deathVFX;

    [Tooltip("Damage made when this target is damaged")]
    [SerializeField] protected AudioClip damagedSound;
    [Tooltip("Damage made when this target is killed")]
    [SerializeField] protected AudioClip deathSound;

    [Header("Drop Stuff")]
    [SerializeField] private List<DroppableQuantity> dropList;

    /// <summary>
    /// Audiosource for this target
    /// </summary>
    protected AudioSource audioSource;

    private void Awake()
    {
        _healthManager = GetComponent<HealthManager>();
        // If initialization of health manager fails, destroy itself
        if(_healthManager == null || !_healthManager.Init())
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
        if(_killed) return;

        if (damagedSound != null)
            AudioSource.PlayClipAtPoint(damagedSound, _center.position);

        if (!_killed && _healthManager.Damage(dmg))
        {
            if (_unkillable)
                return;

            _killed = true;
            KillTarget();
        }
            
    }

    /// <summary>
    /// Kill this target. Default with little functionality
    /// </summary>
    protected virtual void KillTarget()
    {
        if (deathSound != null)
            AudioSource.PlayClipAtPoint(deathSound, _center.position);

        if (_deathVFX != null)
            Instantiate(_deathVFX, _center.position, Quaternion.identity);

        foreach (DroppableQuantity obj in dropList)
        {
            SpawnDrops(obj.spawnObject, obj.dropChance, obj.quantityRange);
        }

        // Debug.Log($"Entity {gameObject.name} has been killed but does not have its own kill function, destroying self.");
        Destroy(gameObject);
    }

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

    #endregion
}
