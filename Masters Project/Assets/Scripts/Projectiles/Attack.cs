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

    /// <summary>
    /// Keep track of hit targets
    /// Track transform roots to ensure it doesnt hit multiple colliders
    /// </summary>
    protected List<Transform> hitTargets;

    protected virtual void Awake()
    {
        hitTargets = new List<Transform>();
    }

    /// <summary>
    /// Deal damage to the given target, if possible
    /// </summary>
    /// <param name="_targetObj">Target to try and deal damage to </param>
    /// <returns>Whether damage was successfully dealt</returns>
    protected virtual bool DealDamage(Transform _targetObj)
    {
        Target target = _targetObj.GetComponent<Target>();
        Transform parentPointer = _targetObj.parent;
        while (target == null)
        {
            if(parentPointer == null)
            {
                return false;
            }
            else
            {
                target = parentPointer.GetComponent<Target>();
                parentPointer = parentPointer.parent;
            }
        }


        // Check if the target can be damaged
        if (target != null && !hitTargets.Contains(target.transform) && !target.Killed())
        {
            // Damage target, prevent multi damaging
            target.RegisterEffect(damage);
            //hitTarget = true;
            hitTargets.Add(target.transform.root);
            return true;
        }
        // otherwise if no target, return false bc no damage dealt
        else
            return false;
    }

    /// <summary>
    /// Deal damage to the targets it hits
    /// </summary>
    /// <param name="other">Object it hit</param>
    private void OnTriggerEnter(Collider other)
    {
        Hit(transform.position);
        DealDamage(other.transform);
    }

    /// <summary>
    /// What visually happens when this attack hits something
    /// These effects should happen on the impact point
    /// </summary>
    protected abstract void Hit(Vector3 impactPoint);

    /// <summary>
    /// Activate this attack
    /// </summary>
    public abstract void Activate();

    /// <summary>
    /// Call a hit function, public so others can call it
    /// </summary>
    public virtual void ScriptCallHit(Transform target)
    {
        Hit(target.position);
    }
}
