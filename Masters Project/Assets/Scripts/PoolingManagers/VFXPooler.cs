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

[System.Serializable]
public struct PrefabPool
{
    public GameObject prefab;
    public int size;
    public int maxSize;
    public int incrementRate;
    public bool recycleOldest;
}

public class VFXPooler : MonoBehaviour
{
    /// <summary>
    /// Global reference to VFX pooler
    /// </summary>
    public static VFXPooler instance;

    /// <summary>
    /// Dictionary for pools
    /// KEY - Hashcode of prefab name
    /// VALUE - Pool object handling specfics
    /// </summary>
    private Dictionary<int, Pool> pools;

    [Tooltip("All prefabs to initialize at start")]
    [SerializeField] private PrefabPool[] VFXPools;

    private void Start()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitPools();
            onSceneChange.OnEventRaised += ReturnAllToPool;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    /// <summary>
    /// Initialize each pool listed in VFXPool
    /// </summary>
    private void InitPools()
    {
        pools = new Dictionary<int, Pool>();

        Pool tempPool;
        foreach (var p in VFXPools)
        {
            tempPool = new Pool(p.prefab, p.size, p.maxSize, p.incrementRate, p.recycleOldest, transform);
            pools.Add(tempPool.Key(), tempPool);
        }
    }
    
    /// <summary>
    /// Request a VFX from the pool
    /// </summary>
    /// <param name="vfxPrefab">VFX prefab being requested</param>
    /// <returns>Instanced VFX</returns>
    public GameObject GetVFX(GameObject vfxPrefab)
    {
        if (vfxPrefab == null)
            return null;

        int id = vfxPrefab.name.GetHashCode();
        if(pools.ContainsKey(id))
        {
            return pools[id].Pull();
        }
        else
        {
            Debug.Log($"[VFXPooler] {vfxPrefab.name} was requested, but not put in the pool!");
            return null;
        }
    }

    /// <summary>
    /// Return the VFX instance to the pool
    /// </summary>
    /// <param name="vfxInstance">instance of the VFX</param>
    /// <returns>Whether the object was returned</returns>
    public bool ReturnVFX(GameObject vfxInstance)
    {
        int id = vfxInstance.name.GetHashCode();
        if (pools.ContainsKey(id))
        {
            vfxInstance.transform.parent= transform;
            vfxInstance.transform.position = Vector3.zero;
            pools[id].Return(vfxInstance);
            return true;
        }
        else
        {
            // if not in pool, just destroy it
            Destroy(vfxInstance);
            return false;
        }
            
    }

    public bool HasPool(GameObject check)
    {
        return pools.ContainsKey(check.name.GetHashCode());
    }

    [SerializeField] ChannelVoid onSceneChange;

    /// <summary>
    /// Return all objects to the pool instantly. Defined by children
    /// </summary>
    private void ReturnAllToPool()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i) != null)
            {
                ReturnVFX(transform.GetChild(i).gameObject);
            }
        }
    }
    private void OnDestroy()
    {
        onSceneChange.OnEventRaised -= ReturnAllToPool;
    }
}
