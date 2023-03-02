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
    public override void Activate()
    {
        if (soundEffect != null)
            transform.PlayClip(soundEffect);
    }

    public override void Deactivate()
    {
        return;
    }
}
