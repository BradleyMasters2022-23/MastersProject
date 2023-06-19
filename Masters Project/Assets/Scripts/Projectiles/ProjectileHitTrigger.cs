using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class ProjectileHitTrigger : Projectile
{
    [Header("On Hit Events")]
    [SerializeField] UnityEvent onHitEvent;

    /// <summary>
    /// When applying damage, trigger any on hit events
    /// </summary>
    /// <param name="target">target to apply to </param>
    /// <param name="damagePoint">point damage was dealt</param>
    protected override void ApplyDamage(Transform target, Vector3 damagePoint)
    {
        onHitEvent?.Invoke();
        base.ApplyDamage(target, damagePoint);
    }
}
