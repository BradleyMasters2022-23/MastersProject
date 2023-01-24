using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIThreatIndicator : MonoBehaviour
{
    [SerializeField] private GameObject arrow;

    private List<GameObject> enemyList;

    public RectTransform indicatorDisplay;

    private void Awake()
    {
        enemyList = new List<GameObject>();
    }

    private void Update()
    {
        // Check for null references (killed enemies), remove them
        for (int i = enemyList.Count - 1; i >= 0; i--)
        {
            if (enemyList[i] == null)
            {
                enemyList.RemoveAt(i);
            }
        }
        enemyList.TrimExcess();

        // get enemies
        EnemyHealth[] enemies = FindObjectsOfType<EnemyHealth>();

        // Check list for new enemies. Add new pointers for each new enemy
        foreach (EnemyHealth enemy in enemies)
        {
            if (!enemyList.Contains(enemy.gameObject))
            {
                enemyList.Add(enemy.gameObject);
                GameObject a = Instantiate(arrow, indicatorDisplay);
                a.GetComponent<UIEnemyPointer>().SetTarget(enemy.gameObject);
            }
        }
    }

}
