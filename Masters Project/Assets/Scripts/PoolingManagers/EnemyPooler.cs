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
    /// Key : Core prefab reference object
    /// Value : List of spawned enemy prefabs
    /// </summary>
    private Dictionary<EnemySO, Pool> pool;

    private void Start()
    {
        Init();
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
            pool[type].Return(enemyReturn);
        }
        else
        {
            Debug.Log($"[EnemyPooler] Pool of enemy {enemyReturn.name} is trying to be returned but does not exist!");
        }
    }
}
