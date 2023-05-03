using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDebugger : MonoBehaviour
{

    [SerializeField] AudioSource s;


    // Update is called once per frame
    void Update()
    {
        bool loop = s.loop;
        float time = s.time;
        bool playing = s.isPlaying;

        //Debug.Log($"Playing : {playing} | Looping : {loop} | Time : {time}");
    }
}
