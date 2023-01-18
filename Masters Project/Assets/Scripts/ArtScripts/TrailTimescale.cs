using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailTimescale : MonoBehaviour
{
    private TrailRenderer ren;
    private float trailTime;
    private float pauseTime;

    private void Awake()
    {
        ren = GetComponent<TrailRenderer>();
        if(ren is null)
            Destroy(this);

        trailTime = ren.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(TimeManager.WorldTimeScale == 0 && ren.emitting)
        {
            pauseTime = Time.time;
            ren.time = Mathf.Infinity;
            ren.emitting = false;
        }
        else if(TimeManager.WorldTimeScale != 0 && !ren.emitting)
        {
            ren.time = (Time.time - pauseTime) + trailTime;

            ren.emitting=true;
        }
    }
}
