using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootPoint : MonoBehaviour
{
    private GameObject indicatorEffect;
    private GameObject muzzleFlashEffect;

    private ParticleSystem[] indicatorSystems;
    private ParticleSystem[] muzzleFlashSystems;

    private void Initialize(GameObject indVFX, GameObject mzfVFX)
    {
        indicatorEffect = Instantiate(indVFX, transform);
        indicatorSystems = indicatorEffect.GetComponentsInChildren<ParticleSystem>(true);

        muzzleFlashEffect = Instantiate(mzfVFX, transform);
        muzzleFlashSystems = muzzleFlashEffect.GetComponentsInChildren<ParticleSystem>(true);
    }

    public void ActivateIndicator()
    {

    }
}
