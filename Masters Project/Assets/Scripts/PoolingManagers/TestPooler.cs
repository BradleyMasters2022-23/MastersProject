using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPooler : MonoBehaviour
{
    public EnemySO[] enemiesToSpawn;
    public float delay;
    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(delay);

        for(int i = 0; i < enemiesToSpawn.Length; i++)
        {
            SpawnEnemy(enemiesToSpawn[i]);
            yield return new WaitForSecondsRealtime(delay);
        }
    }

    private void SpawnEnemy(EnemySO e)
    {
        GameObject enemyToSpawn = EnemyPooler.instance.RequestEnemy(e);
        enemyToSpawn.transform.position = transform.position;
    }
}
