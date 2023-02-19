using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearSpawnManager : MonoBehaviour
{
    public static LinearSpawnManager instance;

    [Header("===Default Easy Relative Size===")]
    [SerializeField] public Vector2Int easyNormalBatchRange;
    [SerializeField] public Vector2Int easyEliteBatchRange;
    [SerializeField] public bool scaleEasyEliteSize;


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

    public Dictionary<EnemySO, int>[] RequestBatch(RelativeDifficulty[] waves, float mod)
    {
        // create new dictionary array that will store all enemy data
        Dictionary<EnemySO, int>[] allWaves = new Dictionary<EnemySO, int>[waves.Length];

        // populate each wave with the given data
        for (int i = 0; i < allWaves.Length; i++)
        {
            allWaves[i] = CreateWave(waves[i], mod);
        }

        // DEBUG - Print out the data to check the info
        for (int i = 0; i < allWaves.Length; i++)
        {
            Debug.Log($"Wave {i} difficulty {waves[i]} is:");
            foreach (var data in allWaves[i])
            {
                Debug.Log(data);
            }
        }

        // Return all wave data to requester
        return allWaves;
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
        if(scaleEasyEliteSize)
            eliteNum += Mathf.FloorToInt(roomNumber / eliteBatchIncrementRate);

        // Adjust for room size, scaling up
        normNum = Mathf.CeilToInt(normNum * sizeMod);
        eliteNum = Mathf.CeilToInt(eliteNum * sizeMod);

        // create spawn dict and a temporary buffer
        Dictionary<EnemySO, int> waveData = new Dictionary<EnemySO, int>();

        // Load normal batch data 
        waveData = LoadInBatches(waveData, normalBatches, normNum);

        // load elite batch data 
        waveData = LoadInBatches(waveData, eliteBatches, eliteNum);
        
        return waveData;
    }

    /// <summary>
    /// Load a certain number of batches from a pool into a dictionary
    /// </summary>
    /// <param name="batchPool">Pool of batches to choose from</param>
    /// <param name="batchNum">Number of batches to load into it</param>
    /// <param name="waveDict">The dictionary to output the data to</param>
    private Dictionary<EnemySO, int> LoadInBatches(Dictionary<EnemySO, int> waveDict, BatchSO[] batchPool, float batchNum)
    {
        // Populate the dictionary with all requested batches
        for (int i = 0; i < batchNum; i++)
        {
            // Choose a batch to load from
            BatchSO selected = batchPool[Random.Range(0, batchPool.Length)];
            
            // Load over every enemy data from the selected batch, add to dictionary
            foreach (EnemySpawnData enemy in selected.batchData)
            {
                // Randomly choose quantity to add, store in dictionary
                int ranQuantity = Random.Range(enemy.spawnCountRange.x, enemy.spawnCountRange.y + 1);

                if (waveDict.ContainsKey(enemy.enemy))
                {
                    waveDict[enemy.enemy] += ranQuantity;
                }
                else
                {
                    waveDict.Add(enemy.enemy, ranQuantity);
                }
            }
        }

        return waveDict;
    }
}
