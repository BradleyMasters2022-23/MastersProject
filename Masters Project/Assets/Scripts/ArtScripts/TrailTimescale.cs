using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailTimescale : MonoBehaviour, TimeObserver
{
    [SerializeField] private TrailRenderer ren;
    [SerializeField] private float trailTime;
    [SerializeField] private float pauseTime;
    public void OnStop()
    {
        pauseTime = Time.time;
        ren.time = Mathf.Infinity;
        ren.emitting = false;
    }
    public void OnResume()
    {
        Debug.Log("New time: " + Time.time + " - " + pauseTime + " = " + (Time.time - pauseTime + trailTime));
        ren.time = (Time.time - pauseTime) + trailTime;
        ren.emitting = true;
    }

    private void Awake()
    {
        ren = GetComponent<TrailRenderer>();
        if(ren is null)
            Destroy(this);

        trailTime = ren.time;
    }
}
