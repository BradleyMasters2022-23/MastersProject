using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHazardCaller : MonoBehaviour
{
    [SerializeField] BulletHazard[] hazards;
    [SerializeField] bool playOnStart;

    private void Start()
    {
        if(playOnStart)
        {
            PlayAll();
        }
    }

    public void PlayAll()
    {
        foreach (var h in hazards)
            h.Play();
    }
    public void StopAll()
    {
        foreach(var h in hazards)
            h.Stop();
    }
}
