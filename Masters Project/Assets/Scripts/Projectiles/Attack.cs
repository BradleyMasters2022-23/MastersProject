/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 24th, 2022
 * Last Edited - October 24th, 2022 by Ben Schuster
 * Description - Abstract base class for all attacks
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attack : MonoBehaviour
{
    [Header("===== Core Attack Info =====")]

    [Tooltip("Damage this attack deals")]
    [SerializeField] protected int damage;

    //[Tooltip("What layers should this attack ignore")]
    //[SerializeField] protected LayerMask layersToIgnore;

    [Tooltip("Amount of knockback this attack does")]
    [SerializeField] protected float knockback;
    [Tooltip("Whether the knockback is added to the target's velocity or overwrites it.")]
    [SerializeField] protected bool addativeKnockback;

    protected bool hitTarget;

    protected abstract void Awake();

    protected virtual void DealDamage(GameObject _target)
    {
        // Check if the target can be damaged
        Damagable target;
        if(!hitTarget && _target.TryGetComponent<Damagable>(out target))
        {
            // Damage target, prevent multi damaging
            target.Damage(damage);
            hitTarget = true;

            // Check if any knockback should be applied, apply it
            if(knockback > 0)
            {
                target.ApplyKnockback(transform.position, knockback, addativeKnockback);
            }
        }
    }

    /// <summary>
    /// Deal damage to the targets it hits
    /// </summary>
    /// <param name="other">Object it hit</param>
    private void OnTriggerEnter(Collider other)
    {
        Hit();
        DealDamage(other.gameObject);
    }

    /// <summary>
    /// What happens when this attack hits something
    /// </summary>
    protected abstract void Hit();

    /// <summary>
    /// What happens when this attack is activated
    /// </summary>
    public abstract void Activate();

}
