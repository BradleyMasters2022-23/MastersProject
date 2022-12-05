using System.Collections;
using System.Collections.Generic;
using System.Security;
using Unity.VisualScripting;
using UnityEngine;

public class SimpleShoot : AttackTarget
{
    [SerializeField] private GameObject projectile;

    [Tooltip("Number of shots to do. Each shot is per-barrel" +
        "EX - 3 shots with 3 barrels will fire 3 shots 3 times, for 9 total.")]
    [SerializeField] private int numOfShots;

    [Tooltip("What is the time in between each shot")]
    [SerializeField] private float delayBetweenShots;

    /// <summary>
    /// Internal trakcer for the time between each shot
    /// </summary>
    private ScaledTimer shotTimer;

    [Tooltip("All barrels for this enemy to shoot")]
    [SerializeField] private Transform[] shootPoints;
    [Tooltip("The object holding the VFX for player indicator")]
    [SerializeField] private GameObject indicatorVFXPrefab;
    [Tooltip("VFX that plays on each shoot point when a projectile is fired")]
    [SerializeField] private GameObject shootVFXPrefab;

    private ParticleSystem[] indicatorEffects;

    protected override IEnumerator DamageAction()
    {
        shotTimer = new ScaledTimer(delayBetweenShots);

        for(int i = 0; i < numOfShots; i++)
        {
            Shoot(shootPoints[0].position + transform.forward);

            shotTimer.ResetTimer();
            while(!shotTimer.TimerDone())
            {
                yield return null;
            }
        }
    }

    protected override void ShowIndicator()
    {
        // if no effects, try to get them
        if(indicatorEffects is null)
        {
            indicatorEffects = indicatorVFXPrefab.GetComponentsInChildren<ParticleSystem>(true);
        }

        indicatorVFXPrefab.SetActive(true);

        // Tell each one to start
        foreach(ParticleSystem par in indicatorEffects)
        {
            par.Play();
        }
    }

    protected override void HideIndicator()
    {
        // Tell each one to stop
        foreach (ParticleSystem par in indicatorEffects)
        {
            par.Stop();
        }

        indicatorVFXPrefab.SetActive(false);
    }

    private void Shoot(Vector3 targetPos)
    {
        // Spawn projectile for each barrel
        List<GameObject> spawnedProjectiles = new List<GameObject>();
        foreach (Transform barrel in shootPoints)
        {
            GameObject o = Instantiate(projectile, barrel.position, projectile.transform.rotation);
            spawnedProjectiles.Add(o);
        }
        RangeAttack temp = spawnedProjectiles[0].GetComponent<RangeAttack>();

        // Aim shot at target position, activate
        foreach (GameObject shot in spawnedProjectiles)
        {
            shot.transform.LookAt(targetPos);
            shot.GetComponent<RangeAttack>().Activate();
        }
    }
}
