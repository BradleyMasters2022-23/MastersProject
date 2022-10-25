/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 24th, 2022
 * Last Edited - October 24th, 2022 by Ben Schuster
 * Description - Core structure for composing room encounters
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Gameplay/Encounter Data", fileName = "New Encounter Data")]
public class EncounterSO : ScriptableObject
{
    public enum WaveType
    {
        Normal,
        Boss
    }

    [Tooltip("All waves this encounter has")]
    public Wave[] waves;
    [Tooltip("Type of encounter")]
    public WaveType type;

    //[Tooltip("How many enemies remaining in the last wave before spawning the next")]
    //public int contThreshold;

    // public List<UpgradeObject> specialRewards;
}

[System.Serializable]
public class Wave
{
    [Tooltip("The composition of enemies to use in this wave")]
    public EnemyGroup[] waveComposition;
}

[System.Serializable]
public class EnemyGroup
{
    [Tooltip("Enemy prefab")]
    [PropertySpace(SpaceBefore = 3)]
    public GameObject enemy;
    [Tooltip("Amount of enemies to spawn")]
    public int amount;
}