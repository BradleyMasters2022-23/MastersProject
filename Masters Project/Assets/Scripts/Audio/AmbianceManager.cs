using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbianceManager : MonoBehaviour
{

    [SerializeField] private AudioClipSO normalAmbiance;

    [SerializeField] private AudioClipSO timeStopAmbiance;

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
        if (TimeManager.WorldTimeScale == 1 && source.clip != normalAmbiance.GetClip())
        {
            Debug.Log("Starting normal ambaince");

            normalAmbiance.PlayClip(null, source);

            source.loop = true;
            source.Play();
        }
        else if (TimeManager.WorldTimeScale <= 0.2 && source.clip != timeStopAmbiance.GetClip())
        {
            Debug.Log("Starting timestop ambiance");

            timeStopAmbiance.PlayClip(null, source);

            source.loop = true;
            source.Play();
        }
        else if (TimeManager.WorldTimeScale > 0.2f && TimeManager.WorldTimeScale != 1 && source.clip != null)
        {
            Debug.Log("Cleaing ambiance");

            source.Stop();
            source.clip = null;
        }

    }
}
