/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - February 15th, 2023
 * Last Edited - February 15th, 2023 by Ben Schuster
 * Description - SO to represent data and any other generic enemy data needed later on
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public enum EnemyType
{
    Normal,
    Elite
}


[CreateAssetMenu(fileName = "New Enemy SO", menuName = "Encounters/Enemy SO")]
public class EnemySO : ScriptableObject
{
    [Tooltip("The enemy associated with this SO")]
    [AssetsOnly] public GameObject enemyPrefab;
    
    public EnemyType enemyType;

    [Tooltip("Cost to spawn this enemy type")]
    public int spawnCost;

    public int minRoomDepth;
    public int maxRoomDepth;
}
