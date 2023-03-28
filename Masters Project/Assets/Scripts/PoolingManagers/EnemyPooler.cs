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
    public static EnemyPooler instance;

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

    public GameObject RequestEnemy(EnemySO enemyRequest)
    {
        if (pool.ContainsKey(enemyRequest))
        {
            GameObject enemy = pool[enemyRequest].Pull();
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

    public void Return(EnemySO type, GameObject enemyReturn)
    {
        if(pool.ContainsKey(type))
        {
            // do other funcs when being returned
            enemyReturn.GetComponent<EnemyTarget>().ReturnToPool();
            pool[type].Return(enemyReturn);
            enemyReturn.transform.parent = transform;
        }
        else
        {
            Debug.Log($"[EnemyPooler] Pool of enemy {enemyReturn.name} is trying to be returned but does not exist!");
        }
    }
}
