using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Target))]
public class TargetSelfDestruct : MonoBehaviour
{
    [SerializeField] Vector2 selfDestructTime;
    private ScaledTimer t;

    private void OnEnable()
    {
        t= new ScaledTimer(Random.Range(selfDestructTime.x, selfDestructTime.y));
    }

    // Update is called once per frame
    void Update()
    {
        // if timer done, kill target
        if(t.TimerDone())
        {
            GetComponent<Target>().RegisterEffect(9999, transform.position);
            this.enabled= false;
        }
    }
}
