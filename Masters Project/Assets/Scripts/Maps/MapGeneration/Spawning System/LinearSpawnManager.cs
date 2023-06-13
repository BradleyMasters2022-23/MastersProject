/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - February 15th, 2023
 * Last Edited - February 27th, 2023 by Ben Schuster
 * Description - Core linear spawner that manages compiling encounters
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public struct EncounterDifficulty
{
    [Header("Difficulty")]
    [Tooltip("Starting budget for this encounter.")]
    public int baseBudget;
    [Tooltip("Depth modifier to the budget. Added once for each room past the first room")]
    public float budgetDepthMod;
    
    [Tooltip("Threshold for maximum budget randomizer. Added multiplied. " +
        "EX: 0.15 will randomly increase OR decrease the encounter budget by a maximum of 15%")]
    [Range(0, 0.5f)] public float randomizePercentageThreshold;
    
    [Tooltip("How likely is an elite to be spawned when first selected.")]
    [Range(0, 1)] public float eliteSpawnChance;

    /// <summary>
    /// Get the budget for this encounter based on depth
    /// </summary>
    /// <param name="depth">current depth of player runs</param>
    /// <returns>randomized budget</returns>
    public int GetBudget(int depth, float globalMod = 0)
    {
        return (int)
            ( baseBudget + Mathf.CeilToInt((budgetDepthMod*globalMod) * depth)
            * (1 + Random.Range(-randomizePercentageThreshold, randomizePercentageThreshold)));
    }
}

public class LinearSpawnManager : MonoBehaviour
{
    public static LinearSpawnManager instance;

    [Tooltip("A scaling modifier based on depth that is applied to all encounters." +
        "Use this to quickly buff or nerf enemy scaling intensity." +
        "Will be a percentage multiplied to all other scaling.")]
    [SerializeField] private float globalDepthScalingBuff;

    [Tooltip("Current number of combat encounters completed")]
    [SerializeField] private int combatRoomCount = 0;
    [Tooltip("All possible enemies to spawn")]
    [SerializeField] private EnemySO[] spawnableEnemies;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            combatRoomCount = 0;
        }
        else
        {
            Destroy(this);
            return;
        }
    }

    public void IncrementDifficulty()
    {
        combatRoomCount++;
    }

    public Dictionary<EnemySO, int>[] RequestBatch(EncounterDifficulty[] waves)
    {
        if (waves == null)
            return null;

        // create new dictionary array that will store all enemy data
        Dictionary<EnemySO, int>[] allWaves = new Dictionary<EnemySO, int>[waves.Length];

        // populate each wave with the given data
        for (int i = 0; i < allWaves.Length; i++)
        {
            allWaves[i] = CreateWave(waves[i]);
        }

        // DEBUG - Print out the data to check the info
        //for (int i = 0; i < allWaves.Length; i++)
        //{
        //    // Debug.Log($"Wave {i} difficulty {waves[i]} is:");
        //    foreach (var data in allWaves[i])
        //    {
        //        Debug.Log(data);
        //    }
        //}

        // Return all wave data to requester
        return allWaves;
    }

    private Dictionary<EnemySO, int> CreateWave(EncounterDifficulty difficulty)
    {
        Dictionary<EnemySO, int> waveData = new Dictionary<EnemySO, int>();

        // Get the budget on the wave based on combat room count
        int budget = difficulty.GetBudget(MapLoader.instance.PortalDepth(), globalDepthScalingBuff);
        // Debug.Log($"Budget for wave is : {budget}");

        // Create buffer lists, useful to reduce iterations later
        List<EnemySO> usableEnemies = spawnableEnemies.ToList();

        int c = 0;
        while(budget > 0 && usableEnemies.Count > 0)
        {
            // Roll to select an enemy
            int randIndex = Random.Range(0, usableEnemies.Count);

            // If an enemy can be used, add to the dict and subtract budget
            if (EnemyUsable(usableEnemies[randIndex], budget))
            {
                // Roll to see if an elite should spawn. If it fails, do a new roll
                if(usableEnemies[randIndex].enemyType == EnemyType.Elite && difficulty.eliteSpawnChance != 1)
                {
                    float eliteRoll = Random.Range(0f, 1f);
                    if (difficulty.eliteSpawnChance < eliteRoll)
                    {
                        // If elite spawn chance is 0, then remove from list
                        if(difficulty.eliteSpawnChance <= 0)
                        {
                            usableEnemies.RemoveAt(randIndex);
                            usableEnemies.TrimExcess();
                        }
                        continue;
                    }
                        
                }

                // Add the enemy to the spawn dictionary
                if (waveData.ContainsKey(usableEnemies[randIndex]))
                {
                    waveData[usableEnemies[randIndex]]++;
                }
                else
                {
                    waveData.Add(usableEnemies[randIndex], 1);
                }
                budget -= usableEnemies[randIndex].spawnCost;
            }
            // If an enemy can't be used, take it out to reduce repeats on future loops
            else
            {
                usableEnemies.RemoveAt(randIndex);
                usableEnemies.TrimExcess();
            }

            // Failsafe loop check
            c++;
            if(c > 1000)
            {
                Debug.LogError("Linear Spawn Manager's 'CreateWave' function is stuck!!!");
                break;
            }
        }

        // Debug.Log($"The remainder budget is : {budget}");

        return waveData;
    }

    /// <summary>
    /// Whether or not this enemy type can be used
    /// </summary>
    /// <param name="enemy">Enemy data to check</param>
    /// <param name="remainingBudget">Remaining budget for enemies</param>
    /// <returns>Whether this enemy type can be used</returns>
    private bool EnemyUsable(EnemySO enemy, int remainingBudget)
    {
        bool validDepth = (combatRoomCount >= enemy.minRoomDepth && combatRoomCount <= enemy.maxRoomDepth);
        bool validBudget = enemy.spawnCost <= remainingBudget;

        return validDepth && validBudget;
    }

    /// <summary>
    /// Load a certain number of batches from a pool into a dictionary - OBSOLETE
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

    public int GetCombatRoomCount()
    {
        return combatRoomCount;
    }
}
