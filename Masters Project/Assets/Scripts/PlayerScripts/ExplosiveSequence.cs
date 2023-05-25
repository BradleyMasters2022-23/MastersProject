/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - May 25th, 2022
 * Last Edited - May 25, 2022 by Ben Schuster
 * Description - A sequence that can be used to chain multiple independent effects
 * together without hard coding
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SpawnEvent
{
    public string name;
    public GameObject spawnObj;
    public Vector3 spawnOffset;
    public float startDelay;
    public float endDelay;

}

public class ExplosiveSequence : TimeAffectedEntity
{
    [SerializeField] private List<SpawnEvent> detonateSequence;

    public void Start()
    {
        StartCoroutine(Detonate());
    }

    private IEnumerator Detonate()
    {
        // prepare a timer to reuse 
        LocalTimer timer = GetTimer(0);

        // Go through each step in the sequence
        foreach(SpawnEvent e in detonateSequence)
        {
            // Allow for start delay
            timer.ResetTimer(e.startDelay);
            yield return new WaitUntil(timer.TimerDone);

            // Spawn object, apply offset
            Transform obj = Instantiate(e.spawnObj, transform.position, transform.rotation).transform;
            obj.position += e.spawnOffset;

            // Allow for end delay
            timer.ResetTimer(e.endDelay);
            yield return new WaitUntil(timer.TimerDone);
        }

        // Once over, destroy self
        Destroy(gameObject);
        yield return null;
    }
}
