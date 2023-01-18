/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - November 4th, 2022
 * Last Edited - November 4th, 2022 by Ben Schuster
 * Description - Manages ensuring each enemy in a scene is tracked with a pointer
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIndicator : MonoBehaviour
{
    /// <summary>
    /// Arrow object used to point at an enemy
    /// </summary>
    [SerializeField] private GameObject arrow;

    /// <summary>
    /// list of enemies currently being traccked by an arrow
    /// </summary>
    private List<GameObject> enemyList;

    private void Awake()
    {
        enemyList = new List<GameObject>();
    }

    private void FixedUpdate()
    {
        // Check for null references (killed enemies), remove them
        for(int i = enemyList.Count-1; i >= 0; i--)
        {
            if(enemyList[i] == null)
            {
                enemyList.RemoveAt(i);
            }
        }
        enemyList.TrimExcess();

        // get enemies
        EnemyHealth[] enemies = FindObjectsOfType<EnemyHealth>();

        // Check list for new enemies. Add new pointers for each new enemy
        foreach(EnemyHealth enemy in enemies)
        {
            if(!enemyList.Contains(enemy.gameObject))
            {
                enemyList.Add(enemy.gameObject);
                GameObject a = Instantiate(arrow, gameObject.transform);
                a.GetComponent<EnemyPointer>().SetTarget(enemy.gameObject);
            }
        }

    }
}
