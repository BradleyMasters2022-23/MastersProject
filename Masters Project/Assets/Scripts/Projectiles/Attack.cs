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
using Sirenix.OdinInspector;
using UnityEngine.UIElements;

public abstract class Attack : MonoBehaviour
{
    [Header("===== Core Attack Info =====")]

    [Tooltip("Damage this attack deals")]
    [SerializeField] protected int damage;
    [SerializeField] protected int playerDamage;

    [Tooltip("Amount of knockback this attack does")]
    [SerializeField] protected bool knockback;

    [HideIf("@this.knockback == false")]
    [SerializeField] private float horizontalKnockback;
    [HideIf("@this.knockback == false")]
    [SerializeField] private float verticalKnockback;
    [HideIf("@this.knockback == false")]
    [SerializeField] private float playerHorizontalKnockback;
    [HideIf("@this.knockback == false")]
    [SerializeField] private float playerVerticalKnockback;
    

    [SerializeField] protected bool affectedByTimestop;

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
    /// Deal damage and knockback to the given target, if possible
    /// </summary>
    /// <param name="_targetObj">Target to try and deal damage to </param>
    /// <returns>Whether damage was successfully dealt</returns>
    protected virtual bool DealDamage(Transform _targetObj)
    {
        return DealDamage(_targetObj, transform.position);
    }

    /// <summary>
    /// Deal damage and knockback to the given target, if possible
    /// </summary>
    /// <param name="_targetObj">target object to deal damage to</param>
    /// <param name="damagePoint">override point of damage for knockback</param>
    /// <returns></returns>
    protected virtual bool DealDamage(Transform _targetObj, Vector3 damagePoint)
    {
        Target target = _targetObj.GetComponent<Target>();
        Transform parentPointer = _targetObj.parent;
        while (target == null)
        {
            if (parentPointer == null)
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
            float dmg;
            float horKnockback;
            float verKnockback;

            // Determine which profile of damage and knockback to use
            if(target.GetType() == typeof(PlayerTarget))
            {
                dmg = playerDamage;
                horKnockback = playerHorizontalKnockback;
                verKnockback= playerVerticalKnockback;
            }
            else
            {
                dmg = damage;
                horKnockback = horizontalKnockback;
                verKnockback = verticalKnockback;
            }

            target.RegisterEffect(dmg);
            if (knockback && horKnockback + verKnockback > 0)
            {
                target.Knockback(horKnockback, verKnockback, damagePoint);
            }

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
        Vector3 hitNormal;

        if (other.GetType() == typeof(MeshCollider) && !other.GetComponent<MeshCollider>().convex)
            hitNormal = -transform.forward;
        else
            hitNormal = transform.position - other.ClosestPoint(transform.position);

        Hit(transform.position, hitNormal.normalized);
        DealDamage(other.transform);
    }

    /// <summary>
    /// What visually happens when this attack hits something
    /// These effects should happen on the impact point
    /// </summary>
    protected abstract void Hit(Vector3 impactPoint, Vector3 hitNormal);

    /// <summary>
    /// Activate this attack
    /// </summary>
    public abstract void Activate();
}
