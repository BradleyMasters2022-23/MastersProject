/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - March 6th, 2022
 * Last Edited - March 6th, 2022 by Ben Schuster
 * Description - Simple script that requests new music when loading into scene
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MusicSelectOnAwake : MonoBehaviour
{
    [EnumPaging]
    [SerializeField] private Music music;
    [SerializeField] private float loadTime;

    // Start is called before the first frame update
    void Start()
    {
        BackgroundMusicManager i = BackgroundMusicManager.instance;
        if (i != null)
            i.SetMusic(music, loadTime);
    }
}
