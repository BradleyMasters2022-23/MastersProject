/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - February 22nd, 2022
 * Last Edited - February 22nd, 2022 by Ben Schuster
 * Description - Concrete indicator implementation for VFX
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

    private void Awake()
    {
        // initialize prefab and get reference to visual effect component
        VFXPrefab = Instantiate(VFXPrefab, transform);
        VFXPrefab.transform.localScale *= scale;
        VFXPrefab.SetActive(false);
        vfxSystem = VFXPrefab.GetComponentInChildren<VisualEffect>(true);
        if (vfxSystem != null)
            vfxSystem.Stop();
    }

    public override void Activate()
    {
        if (vfxSystem == null)
            return;

        VFXPrefab.SetActive(true);
        vfxSystem.Play();
    }

    public override void Deactivate()
    {
        if (vfxSystem == null)
            return;

        VFXPrefab.SetActive(false);
        vfxSystem.Stop();
    }
}
