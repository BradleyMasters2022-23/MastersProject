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

    private List<GameObject> enemyList;

    public RectTransform indicatorDisplay;

    public float maxDistance = Mathf.Infinity;

    private void Awake()
    {
        enemyList = new List<GameObject>();
    }

    private void Update()
    {
        // Check for null references or deactivated (killed enemies), remove them
        for (int i = enemyList.Count - 1; i >= 0; i--)
        {
            if (enemyList[i] == null || !enemyList[i].gameObject.activeInHierarchy)
            {
                enemyList.RemoveAt(i);
            }
        }
        enemyList.TrimExcess();

        // get enemies
        EnemyTarget[] enemies = FindObjectsOfType<EnemyTarget>();

        // Check list for new enemies. Add new pointers for each new enemy
        foreach (EnemyTarget enemy in enemies)
        {
            if (!enemyList.Contains(enemy.gameObject)
                && enemy.gameObject.activeInHierarchy)
            {
                enemyList.Add(enemy.gameObject);
                GameObject a = Instantiate(arrow, indicatorDisplay);
                a.GetComponent<UIEnemyPointer>().SetTarget(enemy.Center.gameObject, 
                    indicatorDisplay.sizeDelta.y / 2, maxDistance);
            }
        }
    }

}
