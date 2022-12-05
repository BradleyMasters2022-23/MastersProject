using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private GameObject indicatorObject;
    [Tooltip("VFX that plays on each shoot point when a projectile is fired")]
    [SerializeField] private GameObject shootVFX;

    protected override IEnumerator DamageAction()
    {
        shotTimer = new ScaledTimer(delayBetweenShots);

        for(int i = 0; i < numOfShots; i++)
        {
            Shoot(transform.position + transform.forward);

            shotTimer.ResetTimer();
            while(!shotTimer.TimerDone())
            {
                yield return null;
            }
        }
    }

    protected override void ShowIndicator()
    {
        
    }
    protected override void HideIndicator()
    {
        
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
}
