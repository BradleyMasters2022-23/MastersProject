using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EnemySpawn
{
    public EnemySO enemyToSpawn;
    public int amountToSpawn;
}
[System.Serializable]
public struct CustomWave
{
    public EnemySpawn[] spawns;
}

public class SpawnTriggerCustomWaves : SpawnTriggerField
{
    [Header("Custom Wave")]
    [SerializeField] private List<CustomWave> customWaves;

    /// <summary>
    /// Build waves based on the custom data passed in
    /// </summary>
    /// <returns></returns>
    protected override Dictionary<EnemySO, int>[] BuildWaves()
    {
        // Generate encounter structure
        Dictionary<EnemySO, int>[] customEncounter = new Dictionary<EnemySO, int>[customWaves.Count];

        // Loop through all waves, convert data to dictionary format
        for(int i = 0; i < customWaves.Count; i++)
        {
            Dictionary<EnemySO, int> customWave = new Dictionary<EnemySO, int>();
            customEncounter[i] = customWave;

            foreach (EnemySpawn data in customWaves[i].spawns)
            {
                customWave.Add(data.enemyToSpawn, data.amountToSpawn);
            }
        }

        return customEncounter;
    }
}
