/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - February 8th, 2022
 * Last Edited - February 8th, 2022 by Ben Schuster
 * Description - Threat indicator for UI, used for the Radar
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIThreatIndicator : MonoBehaviour
{
    [SerializeField] private GameObject arrow;

    private EnemyPooler enemyPooler;

    private List<EnemyTarget> enemyList;

    public RectTransform indicatorDisplay;

    public float maxDistance = Mathf.Infinity;

    private void Awake()
    {
        enemyList = new List<EnemyTarget>();
    }

    private void Update()
    {
        // Check for null references or deactivated (killed enemies), remove them
        for (int i = enemyList.Count - 1; i >= 0; i--)
        {
            if (enemyList[i] == null || enemyList[i].Killed())
            {
                enemyList.RemoveAt(i);
            }
        }

        // get enemies
        if (enemyPooler == null)
        {
            enemyPooler = EnemyPooler.instance;
            return;
        }
        EnemyTarget[] enemies = enemyPooler.GetComponentsInChildren<EnemyTarget>();

        // Check list for new enemies. Add new pointers for each new enemy
        foreach (EnemyTarget enemy in enemies)
        {
            if (!enemyList.Contains(enemy)
                && !enemy.Killed())
            {
                enemyList.Add(enemy);
                GameObject a = Instantiate(arrow, indicatorDisplay);
                a.GetComponent<UIEnemyPointer>().SetTarget(enemy, 
                    indicatorDisplay.sizeDelta.y / 2, maxDistance);
            }
        }
    }

}
