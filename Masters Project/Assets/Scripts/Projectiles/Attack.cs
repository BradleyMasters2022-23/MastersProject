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

[System.Serializable]
public class TeamDmgProfile
{
    public Team team;
    public bool active = true;
    public float damage;
    public float verticalKnockback;
    public float horizontalKnockback;
    public bool canKill = true;

    public void ToggleActive()
    {
        active = !active;
    }
}

[System.Serializable]
public class TeamDamage
{
    [TableList]
    [SerializeField] List<TeamDmgProfile> damageProfiles;

    public TeamDmgProfile GetTeam(Team team)
    {
        foreach(TeamDmgProfile profile in damageProfiles)
        {
            if(profile.team == team)
                return profile;
        }
        return null;
    }

    public void FlipTeamActive(Team team)
    {
        foreach (TeamDmgProfile profile in damageProfiles)
        {
            if (profile.team == team)
            {
                profile.ToggleActive();
                return;
            }
        }
    }
}

public abstract class Attack : TimeAffectedEntity
{
    [Header("===== Core Info =====")]

    [Tooltip("Damage this attack deals")]
    [SerializeField] protected bool dealDamage;
    [HideIf("@this.dealDamage == false")]
    [SerializeField] protected float damage;
    [HideIf("@this.dealDamage == false")]
    [SerializeField] protected float playerDamage;

    [Space(5)]
    [Tooltip("Whether or not this spawns another object on end")]
    [SerializeField] protected bool spawnProjectileOnEnd;
    [HideIf("@this.spawnProjectileOnEnd == false")]
    [Tooltip("Projectile to spawn on end")]
    [SerializeField] protected GameObject onEndPrefab;

    [Space(5)]
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

    /// <summary>
    /// Keep track of hit targets
    /// Track transform roots to ensure it doesnt hit multiple colliders
    /// </summary>
    protected List<Target> hitTargets;

    protected virtual void Awake()
    {
        hitTargets = new List<Target>();
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
        IDamagable target = _targetObj.GetComponent<IDamagable>();
        Target targetComp = target?.Target();

        // Check if the target can be damaged
        if (targetComp != null && !hitTargets.Contains(targetComp))
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

            if(dealDamage)
                target.RegisterEffect(dmg);

            if (knockback)
            {
                targetComp.Knockback(horKnockback, verKnockback, damagePoint);
            }

            hitTargets.Add(targetComp);

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
    protected virtual void OnTriggerEnter(Collider other)
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
