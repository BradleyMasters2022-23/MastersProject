/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - March 27th, 2022
 * Last Edited - March 27th, 2022 by Ben Schuster
 * Description - Manages pooling for all enemies
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyPool
{
    public EnemySO enemy;
    public int poolSize;
    public int maxSize;
    public int incrementRate;
}

public class EnemyPooler : MonoBehaviour
{
    /// <summary>
    /// Global instance of enemy pooler
    /// </summary>
    public static EnemyPooler instance;

    [Tooltip("All initial enemy pools to use")]
    [SerializeField] private EnemyPool[] enemyPools;

    /// <summary>
    /// Dictionary for enemies : 
    /// Key : Enemy data SO
    /// Value : Pool of instanced enemies
    /// </summary>
    private Dictionary<EnemySO, Pool> pool;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            Init();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    /// <summary>
    /// Initialize the pool based on starting values
    /// </summary>
    protected void Init()
    {
        pool = new Dictionary<EnemySO, Pool>();

        // Create a new pool for each option
        foreach (EnemyPool p in enemyPools)
        {
            pool.Add(p.enemy, 
                new Pool(p.enemy.enemyPrefab, p.poolSize, p.maxSize, p.incrementRate, transform));
        }
    }

    /// <summary>
    /// Request an enemy from the pool
    /// </summary>
    /// <param name="enemyRequest">Type of enemy to request</param>
    /// <returns>Instanced version of the requested enemy, as a game object</returns>
    public GameObject RequestEnemy(EnemySO enemyRequest)
    {
        if (pool.ContainsKey(enemyRequest))
        {
            GameObject enemy = pool[enemyRequest].Pull();
            if(enemy == null)
            {
                Debug.Log($"ERROR! Enemy from pool was null while requesting {enemyRequest.name}");
                return null;
            }    
            enemy.transform.parent = null;
            enemy.GetComponent<EnemyTarget>().PullFromPool(enemyRequest);
            // TODO - any unique enemy functionality here like stat scaling
            return enemy;
        }
        else
        {
            Debug.Log($"[EnemyPooler] Pool of enemy {enemyRequest} does not exist!");
            return null;
        }
    }

    /// <summary>
    /// Return an enemy to the pool
    /// </summary>
    /// <param name="type">Type of enemy to use (its key)</param>
    /// <param name="enemyReturn">Reference to instanced enemy to return</param>
    /// <returns>Whether or not the enemy was returned to the pool</returns>
    public bool Return(EnemySO type, GameObject enemyReturn)
    {
        if (type == null || enemyReturn == null)
            return false;

        if(pool.ContainsKey(type))
        {
            // do other funcs when being returned
            enemyReturn.GetComponent<EnemyTarget>().ReturnToPool();
            pool[type].Return(enemyReturn);
            enemyReturn.transform.parent = transform;
            return true;
        }
        else
        {
            Debug.Log($"[EnemyPooler] Pool of enemy {enemyReturn.name} is trying to be returned but does not exist!");
            return false;
        }
    }
}
