/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - February 22nd, 2022
 * Last Edited - February 22nd, 2022 by Ben Schuster
 * Description - Concrete indicator implementation for sound
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioIndicator : IIndicator
{
    [SerializeField] AudioClipSO soundEffect;
    [SerializeField] private AudioSource source;
    [SerializeField] private bool overlap; 
    public override void Activate()
    {
        soundEffect.PlayClip(transform, source, overlap);
    }

    public override void Deactivate()
    {
        return;
    }
}
