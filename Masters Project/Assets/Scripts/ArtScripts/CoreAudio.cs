/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - March 2nd, 2022
 * Last Edited - March 2nd, 2022 by Ben Schuster
 * Description - Core static functions for volume and audio contol
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public static class CoreAudio
{
    /// <summary>
    /// Play an audio clip given audio data
    /// </summary>
    /// <param name="origin">The origin of this audio clip</param>
    /// <param name="data">The audio data to use</param>
    /// <param name="destination">The source to play the audio from. If empty, it will create a temporary container</param>
    /// <param name="overlap">Whether to play by itself or overwrite the previous sound effect</param>
    public static void PlayClip(this AudioClipSO data, Transform origin, AudioSource destination = null, bool overlap = false)
    {
        if(data == null)
        {
            // Comment this out when not actively looking for false calls
            Debug.LogError($"Error! {origin.name} requested audio but did not pass in any audio!");
            return;
        }

        // If no destination, create a temporary container
        if (destination == null)
        {
            // Create container and move to original position
            GameObject container = new GameObject($"CoreAudio Oneshot : {data.name}");
            if(origin != null)
                container.transform.position = origin.transform.position;
            destination = container.AddComponent<AudioSource>();
            destination.Stop();

            if(!data.loop)
                GameObject.Destroy(container, data.GetClip().length);
        }
        else
        {
            destination.Stop();
        }

        // Load the destination with the data from the audio SO
        destination.clip = data.GetClip();
        destination.priority = data.priority;
        destination.playOnAwake = false;
        destination.volume = data.volume;
        destination.outputAudioMixerGroup = data.volumeBus;
        destination.pitch = data.GetPitch();
        destination.loop = data.loop;
        destination.spatialBlend = data.spatialBlend;
        destination.minDistance = data.minDistance;
        destination.maxDistance = data.maxDistance;
        destination.rolloffMode = data.distanceMode;
        destination.dopplerLevel = data.doppler;

        // Play audio based on independence setting
        if(overlap && !destination.loop)
        {
            destination.PlayOneShot(destination.clip, destination.volume);
            
        }
        else
        {
            destination.Play();
        }
    }

    /// <summary>
    /// Play an audio clip given audio data
    /// </summary>
    /// <param name="data">audio data to play</param>
    /// <param name="destination">source player. Leave null to create one</param>
    /// <param name="overlap">whether to overlap other audio if in a source</param>
    public static void PlayClip(this AudioClipSO data, AudioSource destination = null, bool overlap = false)
    {
        data.PlayClip(null, destination, overlap);
    }
}
