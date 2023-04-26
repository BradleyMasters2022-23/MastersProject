using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalEnemyManager : GlobalPhysicsManager
{
    public static GlobalEnemyManager instance;

    [SerializeField] private LayerMask enemyHitLayers;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
    }

    /// <summary>
    /// Get the global hit layers
    /// </summary>
    /// <returns>layer mask of all damagable layers</returns>
    public LayerMask GetHitLayers()
    {
        return enemyHitLayers;
    }
}
