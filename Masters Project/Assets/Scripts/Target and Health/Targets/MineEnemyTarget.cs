using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineEnemyTarget : EnemyTarget
{
    [Header("Mine Enemy")]
    [SerializeField] Explosive deathExplosion;

    /// <summary>
    /// When mine enemy dies, do a death explosion
    /// </summary>
    protected override void DestroyObject()
    {
        if(deathExplosion != null)
        {
            Instantiate(deathExplosion.gameObject, _center.position, Quaternion.identity);

        }
        base.DestroyObject();
    }
}
