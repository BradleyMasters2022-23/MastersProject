/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - March 5th, 2022
 * Last Edited - March 5th, 2022 by Ben Schuster
 * Description - Spawner used to test the enemy object pooler 
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPoolTester : MonoBehaviour
{
    public float startDelay;
    public float respawnDelay;

    public EnemySO enemyRequest;

    private void Start()
    {
        InvokeRepeating("RequestEnemy", startDelay, respawnDelay);
    }

    public void RequestEnemy()
    {
        if (EnemyPooler.instance == null) return;


        GameObject enemy = EnemyPooler.instance.RequestEnemy(enemyRequest);
        
        if(enemy == null)
        {
            Debug.Log($"Testing enemy pooler, but enemy {enemyRequest} was not found");
            return;
        }

        enemy.transform.position = transform.position;
        enemy.transform.rotation = transform.rotation;
        enemy.SetActive(true);
    }
}
