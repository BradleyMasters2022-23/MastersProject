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

    public virtual void NewKnockback(Vector3 _origin, float _horizontalForce, float _verticalForce, float radius)
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        if(rb is null) return;

        //Vector3 dir = (transform.position - _origin).normalized;

        //rb.AddExplosionForce(_horizontalForce, _origin, radius, _verticalForce);
        Vector3 dir = (transform.position - _origin).normalized;
        dir.y = 0;

        rb.velocity = Vector3.zero;

        rb.AddForce(dir * _horizontalForce + Vector3.up * _verticalForce, ForceMode.Impulse);

        Debug.DrawLine(rb.position, rb.position + dir * _horizontalForce + Vector3.up * _verticalForce, Color.red, 2f);
    }
}
