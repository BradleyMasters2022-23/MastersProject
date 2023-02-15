using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearSpawnManager : MonoBehaviour
{
    public static LinearSpawnManager instance;

    [Header("===Default Easy Relative Size===")]
    [SerializeField] public Vector2Int easyNormalBatchRange;
    [SerializeField] public Vector2Int easyEliteBatchRange;

    [Header("===Default Medium Relative Size===")]
    [SerializeField] public Vector2Int mediumNormalBatchRange;
    [SerializeField] public Vector2Int mediumEliteBatchRange;
    
    [Header("===Default Hard Relative Size===")]
    [SerializeField] public Vector2Int hardNormalBatchRange;
    [SerializeField] public Vector2Int hardEliteBatchRange;


    [Header("Core Batch Data")]
    [SerializeField] private BatchSO[] normalBatches;
    [SerializeField] private BatchSO[] eliteBatches;



    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
    }

    public int enemyCount;
    public int roomNumber;

    public int normalBatchIncrementRate;
    public int eliteBatchIncrementRate;

    public void RequestBatch(RelativeDifficulty[] waves, SpawnPoint[] spawnPoints, float mod)
    {
        StartCoroutine(SpawnField(waves, spawnPoints, mod));
    }

    private IEnumerator SpawnField(RelativeDifficulty[] waves, SpawnPoint[] spawnPoints, float mod)
    {
        Dictionary<EnemySO, int> spawnPool;

        // Determine difficulty, create batches
        spawnPool = CreateWave(waves[0], mod);

        Debug.Log("The created batch for the first wave is...");
        foreach(var data in spawnPool)
        {
            Debug.Log(data);
        }


        yield return null;
    }

    private Dictionary<EnemySO, int> CreateWave(RelativeDifficulty difficulty, float sizeMod)
    {
        // determine modifier data, apply starting range value
        int normNum;
        int eliteNum;

        switch(difficulty)
        {
            case RelativeDifficulty.Easy:
                {
                    normNum = Random.Range(easyNormalBatchRange.x, easyNormalBatchRange.y+1);
                    eliteNum = Random.Range(easyEliteBatchRange.x, easyEliteBatchRange.y+1);
                    break;
                }
            case RelativeDifficulty.Normal:
                {
                    normNum = Random.Range(mediumNormalBatchRange.x, mediumNormalBatchRange.y + 1);
                    eliteNum = Random.Range(mediumEliteBatchRange.x, mediumEliteBatchRange.y + 1);
                    break;
                }
            case RelativeDifficulty.Hard:
                {
                    normNum = Random.Range(hardNormalBatchRange.x, hardNormalBatchRange.y + 1);
                    eliteNum = Random.Range(hardEliteBatchRange.x, hardEliteBatchRange.y + 1);
                    break;
                }
            default:
                {
                    normNum = 3;
                    eliteNum = 1;
                    break;
                }
        }

        // Adjust for room depth, scaling down
        normNum += Mathf.FloorToInt(roomNumber / normalBatchIncrementRate);
        eliteNum += Mathf.FloorToInt(roomNumber / eliteBatchIncrementRate);

        // Adjust for room size, scaling up
        normNum = Mathf.CeilToInt(normNum * sizeMod);
        eliteNum = Mathf.CeilToInt(eliteNum * sizeMod);

        // create spawn dict and a temporary buffer
        Dictionary<EnemySO, int> waveData = new Dictionary<EnemySO, int>();

        Dictionary<EnemySO, int> dataBuffer;

        // Load normal batch data into buffer, move over to wave data
        dataBuffer = LoadInBatches(normalBatches, normNum);
        foreach(KeyValuePair<EnemySO, int> data in dataBuffer)
        {
            if(waveData.ContainsKey(data.Key))
            {
                waveData[data.Key] += data.Value;
            }
            else
            {
                waveData.Add(data.Key, data.Value);
            }
        }

        // load elite batch data into buffer, move over to wave data
        dataBuffer = LoadInBatches(eliteBatches, eliteNum);
        foreach (KeyValuePair<EnemySO, int> data in dataBuffer)
        {
            if (waveData.ContainsKey(data.Key))
            {
                waveData[data.Key] += data.Value;
            }
            else
            {
                waveData.Add(data.Key, data.Value);
            }
        }
        
        
        return waveData;
    }

    /// <summary>
    /// Load a certain number of batches from a pool into a dictionary
    /// </summary>
    /// <param name="batchPool">Pool of batches to choose from</param>
    /// <param name="batchNum">Number of batches to load into it</param>
    /// <param name="waveData">The dictionary to output the data to</param>
    private Dictionary<EnemySO, int> LoadInBatches(BatchSO[] batchPool, float batchNum)
    {
        Dictionary<EnemySO, int> newDict = new Dictionary<EnemySO, int>();

        for (int i = 0; i < batchNum; i++)
        {
            // Choose a batch to load from
            BatchSO selected = batchPool[Random.Range(0, batchPool.Length)];
            Debug.Log($"Loading batch named : {selected.name}");
            // Load over every enemy data from the selected batch, add to dictionary
            foreach (EnemySpawnData enemy in selected.batchData)
            {
                // Randomly choose quantity to add, store in dictionary
                int ranQuantity = Random.Range(enemy.spawnCountRange.x, enemy.spawnCountRange.y + 1);

                if (newDict.ContainsKey(enemy.enemy))
                {
                    newDict[enemy.enemy] += ranQuantity;
                }
                else
                {
                    newDict.Add(enemy.enemy, ranQuantity);
                }
            }
        }

        return newDict;
    }
}
