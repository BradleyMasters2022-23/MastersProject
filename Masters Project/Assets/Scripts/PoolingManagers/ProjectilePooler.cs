/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - June 5th, 2023
 * Last Edited - June 5th, 2023 by Ben Schuster
 * Description - Pooler for VFX prefabs to help reduce performance impact of VFX spawning
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ProjectilePooler : MonoBehaviour
{
    /// <summary>
    /// Global reference to VFX pooler
    /// </summary>
    public static ProjectilePooler instance;

    /// <summary>
    /// Dictionary for pools
    /// KEY - Hashcode of prefab name
    /// VALUE - Pool object handling specfics
    /// </summary>
    private Dictionary<int, Pool> pools;

    [Tooltip("All prefabs to initialize at start")]
    [SerializeField] private PrefabPool[] projectilePools;

    private void Start()
    {
        // Override pool if a new one is available!
        if(instance != null)
        {
            Debug.Log("Clearing previous pooler");
            Destroy(instance.gameObject);
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        InitPools();
    }

    /// <summary>
    /// Initialize each pool listed in VFXPool
    /// </summary>
    private void InitPools()
    {
        pools = new Dictionary<int, Pool>();

        Pool tempPool;
        foreach (var p in projectilePools)
        {
            tempPool = new Pool(p.prefab, p.size, p.maxSize, p.incrementRate, p.recycleOldest, transform);
            pools.Add(tempPool.Key(), tempPool);
        }
    }
    
    /// <summary>
    /// Request a VFX from the pool
    /// </summary>
    /// <param name="projectilePrefab">VFX prefab being requested</param>
    /// <returns>Instanced VFX</returns>
    public GameObject GetProjectile(GameObject projectilePrefab)
    {
        if (projectilePrefab == null)
            return null;

        int id = projectilePrefab.name.GetHashCode();
        if(pools.ContainsKey(id))
        {
            return pools[id].Pull();
        }
        else
        {
            //Debug.Log($"[VFXPooler] {projectilePrefab.name} was requested, but not put in the pool!");
            return null;
        }
    }

    /// <summary>
    /// Return the VFX instance to the pool
    /// </summary>
    /// <param name="projectileInstance">instance of the VFX</param>
    /// <returns>Whether the object was returned</returns>
    public bool ReturnProjectile(GameObject projectileInstance)
    {
        int id = projectileInstance.name.GetHashCode();
        if (pools.ContainsKey(id))
        {
            pools[id].Return(projectileInstance);
            return true;
        }
        else
            return false;
    }

    /// <summary>
    /// Check whether a projectile is being pooled
    /// </summary>
    /// <param name="check">Prefab to check</param>
    /// <returns></returns>
    public bool HasPool(GameObject check)
    {
        return pools.ContainsKey(check.name.GetHashCode());
    }
}
