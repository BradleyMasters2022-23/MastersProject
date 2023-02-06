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
        Transform parent = _target.transform;
        Target target;

        // continually escelate up for a targetable reference
        while (!parent.TryGetComponent<Target>(out target) && parent.parent != null)
        {
            parent = parent.parent;
        }

        // Check if the target can be damaged
        if(!hitTarget && target != null)
        {
            // Damage target, prevent multi damaging
            target.RegisterEffect(damage);
            hitTarget = true;
        }
    }

    /// <summary>
    /// Trigger the damage effects on a target
    /// </summary>
    /// <param name="other"></param>
    protected void TriggerTarget(Collider other)
    {
        Hit();

        DestroyProjScript test;

        if (other.TryGetComponent<DestroyProjScript>(out test))
            test.DestroyProj();

        DealDamage(other.gameObject);
    }

    /// <summary>
    /// Deal damage to the targets it hits
    /// </summary>
    /// <param name="other">Object it hit</param>
    private void OnTriggerEnter(Collider other)
    {
        TriggerTarget(other);
    }

    /// <summary>
    /// What happens when this attack hits something
    /// </summary>
    protected abstract void Hit();

    /// <summary>
    /// What happens when this attack is activated
    /// </summary>
    public abstract void Activate();

    /// <summary>
    /// Call a hit function, public so others can call it
    /// </summary>
    public virtual void ScriptCallHit()
    {
        Hit();
    }
}
