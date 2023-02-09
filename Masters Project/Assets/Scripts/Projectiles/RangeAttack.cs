/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 24th, 2022
 * Last Edited - October 24th, 2022 by Ben Schuster
 * Description - Abstract base class for all ranged attacks
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RangeAttack : Attack
{
    public enum AttackType
    {
        PROJECTILE,
        HITSCAN,
    }
    [Tooltip("Base speed this attack moves at")]
    [SerializeField] protected float speed;
    [Tooltip("Range of this ranged attack")]
    [SerializeField] protected float range;
    [SerializeField] protected bool shotByPlayer;

    /// <summary>
    /// Base speed this attack moveds at
    /// </summary>
    public float Speed
    {
        get { return speed; }
    }

    /// <summary>
    /// Initialize any necessary variables needing to be passed in. 
    /// </summary>
    /// <param name="_damageMultiplier">multiplier for the damage. % based.</param>
    /// <param name="_speedMultiplier">multiplier for the speed. % based.</param>
    public void Initialize(float _damageMultiplier, float _speedMultiplier, bool _shotByPlayer = false)
    {
        damage *= Mathf.FloorToInt(_damageMultiplier);
        speed *= _speedMultiplier;
        shotByPlayer = _shotByPlayer;
        // Activate the projectile after being initialized
        Activate();
    }

    public void SetMaxRange(float newRange)
    {
        range = newRange;   
    }

    /// <summary>
    /// What happens when this ranged attack ends
    /// </summary>
    protected virtual void End()
    {
        Destroy(gameObject);
    }
}
