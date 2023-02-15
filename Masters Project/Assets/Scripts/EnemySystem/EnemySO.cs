/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - February 15th, 2023
 * Last Edited - February 15th, 2023 by Ben Schuster
 * Description - So to represent data and any other generic enemy data needed later on
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Enemy SO", menuName = "Encounters/Enemy SO")]
public class EnemySO : ScriptableObject
{
    [Tooltip("The enemy associated with this SO")]
    [AssetsOnly] public GameObject enemyPrefab;
}
