/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - February 22nd, 2022
 * Last Edited - April 28, 2022 by Ben Schuster
 * Description - Concrete indicator implementation for VFX, both normal and legacy
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VFXIndicator : IIndicator
{
    [SerializeField] private GameObject VFXPrefab;
    [SerializeField] private float scale = 1;
    private VisualEffect vfxSystem;
    private ParticleSystem[] legacyVFX;

    private void Awake()
    {
        // initialize prefab and get reference to visual effect component
        VFXPrefab = Instantiate(VFXPrefab, transform);
        VFXPrefab.transform.localScale *= scale;

        vfxSystem = VFXPrefab.GetComponentInChildren<VisualEffect>(true);
        if (vfxSystem != null)
            vfxSystem.Stop();

        legacyVFX = VFXPrefab.GetComponentsInChildren<ParticleSystem>(true);
        
        if(legacyVFX.Length > 0)
        {
            foreach(var v in legacyVFX)
            {
                v?.Stop();
            }
        }

        gameObject.SetActive(false);
    }

    public override void Activate()
    {
        gameObject.SetActive(true);

        if (vfxSystem != null)
        {
            vfxSystem.Play();
        }

        if(legacyVFX.Length > 0)
        {
            foreach (var v in legacyVFX)
            {
                v?.Play();
            }
        }
    }

    public override void Deactivate()
    {
        gameObject.SetActive(false);

        if (vfxSystem != null)
        {
            vfxSystem.Stop();
        }

        if (legacyVFX != null && legacyVFX.Length > 0)
        {
            foreach (var v in legacyVFX)
            {
                v?.Stop();
            }
        }
    }
}
