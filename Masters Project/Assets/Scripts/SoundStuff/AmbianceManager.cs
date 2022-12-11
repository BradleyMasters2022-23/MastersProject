using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbianceManager : MonoBehaviour
{

    [SerializeField] private AudioClip normalAmbiance;
    [SerializeField] private float ambianceVolume;

    [SerializeField] private AudioClip timeStopAmbiance;
    [SerializeField] private float timeStopAmbianceVolume;

    private AudioSource source;


    private void Awake()
    {
        if(normalAmbiance == null && timeStopAmbiance == null)
        {
            Destroy(this);
            return;
        }

        source = GetComponent<AudioSource>();
    }


    /// <summary>
    /// Update the ambiance based on timestop
    /// </summary>
    private void Update()
    {
        if (normalAmbiance != null && TimeManager.WorldTimeScale == 1 && source.clip != normalAmbiance)
        {
            //Debug.Log("Starting normal ambaince");

            source.clip = normalAmbiance;
            source.volume = ambianceVolume;

            source.loop = true;
            source.Play();
        }
        else if (timeStopAmbiance != null && TimeManager.WorldTimeScale == 0 && source.clip != timeStopAmbiance)
        {
            //Debug.Log("Starting timestop ambiance");

            source.clip = timeStopAmbiance;
            source.volume = timeStopAmbianceVolume;

            source.loop = true;
            source.Play();
        }
        else if (TimeManager.WorldTimeScale != 0 && TimeManager.WorldTimeScale != 1 && source.clip != null)
        {
            //Debug.Log("Cleaing ambiance");

            source.Stop();
            source.clip = null;
        }

    }
}
