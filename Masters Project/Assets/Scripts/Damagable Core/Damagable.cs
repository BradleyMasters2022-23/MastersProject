/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 21th, 2022
 * Last Edited - October 21th, 2022 by Ben Schuster
 * Description - Base damagable class. Allows for an entity to take damage and die
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Damagable : MonoBehaviour
{
    public enum Team
    {
        PLAYER,
        ENEMY
    }

    [Header("=====Knockback=====")]

    [Tooltip("The maximum vertical knockback this entity can have.")]
    [SerializeField] private float maxVerticalKnockback;

    /// <summary>
    /// Whether or not this entity has been killed
    /// </summary>
    protected bool killed;

    /// <summary>
    /// Deal damage to this entity
    /// </summary>
    public abstract void Damage(int _dmg);

    /// <summary>
    /// Kill this entity
    /// </summary>
    protected abstract void Die();

    /// <summary>
    /// Apply knockback force on the target
    /// </summary>
    /// <param name="_origin">origin position of the knockback</param>
    /// <param name="_force">strength of the knockback</param>
    /// <param name="_additive">Whether to add force or override it</param>
    public virtual void ApplyKnockback(Vector3 _origin, float _force, bool _additive)
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb is null)
            return;

        // Prepare the new velocity vector for knockback
        Vector3 newForce = (transform.position - _origin).normalized * _force;
        
        // If additive, then add to current velocity
        if(_additive)
        {
            newForce += rb.velocity;
        }

        // Check if upper limit for vertical knockback needs to be clamped
        if(newForce.y > maxVerticalKnockback)
        {
            newForce.y = maxVerticalKnockback;
        }

        // Apply new force
        rb.velocity = newForce;
    }

    /// <summary>
    /// Apply a knockback with horizontal and vertial forces, acting more explosive
    /// </summary>
    /// <param name="_origin">origin of knockback</param>
    /// <param name="_horizontalForce">default horizontal force applied to player</param>
    /// <param name="_verticalForce">default vertical force applied to player</param>
    /// <param name="falloff">Curve determining falloff of knockback</param>
    public virtual void ExplosiveKnockback(Vector3 _origin, float scale,
        float _horizontalForce, float _verticalForce, 
        float radius, AnimationCurve falloff)
    {
        // Get RB to apply physics. Exit if no RB
        Rigidbody rb = GetComponent<Rigidbody>();
        if(rb is null) 
            return;

        // Determine direction to launch the object in 
        Vector3 dir = (transform.position - _origin).normalized;
        dir.y = 0;

        // Determine force to apply based on distance to target
        float dist = Mathf.Abs(Vector3.Distance(transform.position, _origin));
        float horKnockback = _horizontalForce;
        float verKnockback = _verticalForce;
        float mod = dist / (radius*scale);
        Debug.Log("Distance to explosion is " + dist);

        horKnockback *= falloff.Evaluate(mod);
        verKnockback *= falloff.Evaluate(mod);

        Debug.Log("modified knockback by " + falloff.Evaluate(mod));

        rb.velocity = Vector3.zero;
        rb.position += Vector3.up * 0.5f;

        Vector3 knockback = dir * horKnockback + Vector3.up * verKnockback;

        rb.AddForce(knockback, ForceMode.Impulse);

        Debug.DrawLine(rb.position, rb.position + knockback, Color.red, 2f);
    }
}
