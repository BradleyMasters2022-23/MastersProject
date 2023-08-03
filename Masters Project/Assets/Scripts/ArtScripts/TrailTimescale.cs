using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailTimescale : MonoBehaviour, TimeObserver
{
    private TrailRenderer ren;
    private float trailTime;
    private float pauseTime;

    public void OnResume()
    {
        ren.time = (Time.time - pauseTime) + trailTime;
        ren.emitting = true;
    }

    public void OnStop()
    {
        pauseTime = Time.time;
        ren.time = Mathf.Infinity;
        ren.emitting = false;
    }

    private void Awake()
    {
        ren = GetComponent<TrailRenderer>();
        if(ren is null)
            Destroy(this);

        trailTime = ren.time;
    }

    //private void OnEnable()
    //{
    //    TimeManager.instance.Subscribe(this);
    //}
    //private void OnDisable()
    //{
    //    TimeManager.instance.UnSubscribe(this);
    //}
}
