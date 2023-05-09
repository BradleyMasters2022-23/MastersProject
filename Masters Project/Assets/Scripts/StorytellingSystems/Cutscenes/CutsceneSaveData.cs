using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class CutsceneSaveData
{
    public List<string> playedCutscenes;
    public List<string> singlePlayCutscenes;

    public CutsceneSaveData()
    {
        playedCutscenes = new List<string>();
        singlePlayCutscenes = new List<string>();
    }

    /// <summary>
    /// Check whether a video has been played once
    /// </summary>
    /// <param name="v">Video to check</param>
    /// <returns>Whether the video has already been placed once</returns>
    public bool HasCutsceneBeenPlayed(VideoClip v)
    {
        string c = v.name;
        return playedCutscenes.Contains(c);
    }
    /// <summary>
    /// Check if a video has already been played AND if it should only be played once
    /// </summary>
    /// <param name="v">Video to check</param>
    /// <returns>Whether the video has been played and its set to play once</returns>
    public bool SinglePlayOnly(VideoClip v)
    {
        string c = v.name;
        return singlePlayCutscenes.Contains(c);
    }

    /// <summary>
    /// Add a cutscene to the played saved data
    /// </summary>
    /// <param name="v">video to check</param>
    /// <param name="singlePlay">whether this video should only be played once per save</param>
    public void PlayCutscene(VideoClip v, bool singlePlay)
    {
        // Add to played cutscenes
        string c = v.name;
        if(!playedCutscenes.Contains(c))
        {
            playedCutscenes.Add(c);
        }

        // If set to play only once, add to list
        if(singlePlay && !singlePlayCutscenes.Contains(c))
        {
            singlePlayCutscenes.Add(c);
        }
    }
}
