/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - February 15th, 2023
 * Last Edited - February 15th, 2023 by Ben Schuster
 * Description - SO for batches of enemies, used to help randomly compose combat encounters
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class EnemySpawnData
{
    [Tooltip("What enemy to spawn")]
    public EnemySO enemy;
    [Tooltip("Range of this enemy to spawn")]
    public Vector2Int spawnCountRange;
}

public enum BatchType
{
    Normal, 
    Elite
}

[CreateAssetMenu(fileName = "New Batch SO", menuName = "Encounters/Batch SO")]
public class BatchSO : ScriptableObject
{
    [Tooltip("All enemies and ranges that make up this batch")]
    public List<EnemySpawnData> batchData;
    [Tooltip("The minimum depth of this room needed for this batch to be usable")]
    public int minimumRoomNumber;
    [Tooltip("The category of this batch")]
    public BatchType type;

    /// <summary>
    /// Whether or not this batch can be used
    /// </summary>
    /// <param name="roomNumber">The current number of rooms the player has completed</param>
    /// <returns>Whether this batch is usable</returns>
    public bool Usable(int roomNumber)
    {
        return roomNumber >= minimumRoomNumber;
    }
}
