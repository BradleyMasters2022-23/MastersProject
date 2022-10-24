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
}
