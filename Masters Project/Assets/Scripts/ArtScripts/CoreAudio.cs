using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public static class CoreAudio
{
    public static void PlayClip(this Transform origin, AudioClipSO data)
    {
        // Create container and move to original position
        GameObject container = new GameObject("CoreAudio Oneshot");
        container.transform.position = origin.transform.position;
        AudioSource source = container.AddComponent<AudioSource>();
        source.Stop();

        // Load the source with the data from the audio SO
        source.clip = data.GetClip();
        source.playOnAwake = false;
        source.volume = data.volume;
        source.outputAudioMixerGroup = data.volumeType;
        source.pitch = data.GetPitch();
        source.loop = data.loop;
        source.spatialBlend = data.spatialBlend;
        source.minDistance= data.minDistance;
        source.maxDistance= data.maxDistance;
        source.rolloffMode = data.distanceMode;
        source.dopplerLevel = data.doppler;

        source.Play();

        // Play the clip, determine self destruct time if not looping
        if (!source.loop)
        {
            GameObject.Destroy(container, source.clip.length);
        }
    }
}
