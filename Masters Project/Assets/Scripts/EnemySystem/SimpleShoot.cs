using Sirenix.OdinInspector.Editor;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security;
using Unity.VisualScripting;
using UnityEngine;

public class SimpleShoot : AttackTarget
{

    [Header("=== Ranged Attack Data ===")]

    [Tooltip("The projectile prefab to use")]
    [SerializeField] private GameObject projectile;
    [Tooltip("All barrels for this enemy to shoot")]
    [SerializeField] private Transform[] shootPoints;
    [Tooltip("Number of shots to do. Each shot is per-barrel" +
        "EX - 3 shots with 3 barrels will fire 3 shots 3 times, for 9 total.")]
    [Space(5)]
    [SerializeField] private int numOfShots;
    [Tooltip("What is the time in between each shot")]
    [SerializeField] private float delayBetweenShots;

    [Space(10)]
    [Tooltip("What is the minimim and maximum strength for leading shots")]
    [SerializeField] private Vector2 leadStrength;

    [Space(10)]
    [Tooltip("What is the range of accuracy for the first shot")]
    [SerializeField] private Vector2 firstShotAccuracy;
    [Tooltip("What is the range of accuracy on this")]
    [SerializeField] private Vector2 normalAccuracyRange;

    [Space(10)]
    [Tooltip("How close in range does the target need to be to still aim at player")]
    [SerializeField] private float visionRange;

    [Header("=== Visuals and Sounds ===")]

    [Tooltip("The object holding the VFX for player indicator")]
    [SerializeField] private GameObject indicatorVFXPrefab;
    [Tooltip("VFX that plays on each shoot point when a projectile is fired")]
    [SerializeField] private GameObject shootVFXPrefab;

    /// <summary>
    /// Get the indicator gameobject references
    /// </summary>
    [SerializeField] private GameObject[] indicators;

    /// <summary>
    /// Internal trakcer for the time between each shot
    /// </summary>
    private ScaledTimer shotTimer;

    

    /// <summary>
    /// Initialize each thing
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        indicators = new GameObject[shootPoints.Length];
        for(int i = 0; i < shootPoints.Length; i++) 
        {
            indicators[i] = Instantiate(indicatorVFXPrefab, shootPoints[i]);
            indicators[i].transform.localPosition= Vector3.zero;
            indicators[i].SetActive(false);
        }
    }

    protected override IEnumerator DamageAction()
    {
        shotTimer = new ScaledTimer(delayBetweenShots);

        for(int i = 0; i < numOfShots; i++)
        {
            // Apply first shot accuracy instead
            if(i == 0)
            {
                Shoot(target, 
                    firstShotAccuracy.x, firstShotAccuracy.y);
            }
            else
            {
                Shoot(target,
                    normalAccuracyRange.x, normalAccuracyRange.y);
            }


            shotTimer.ResetTimer();
            while(!shotTimer.TimerDone())
            {
                yield return null;
            }
        }
    }

    protected override void ShowIndicator()
    {
        // Tell each one to start
        foreach(GameObject indicator in indicators)
        {
            indicator.SetActive(true);

            foreach (ParticleSystem par in indicator.GetComponentsInChildren<ParticleSystem>(true))
            {
                par.Play();
            }

        }
        
    }

    protected override void HideIndicator()
    {
        // Tell each one to stop
        foreach (GameObject indicator in indicators)
        {
            foreach (ParticleSystem par in indicator.GetComponentsInChildren<ParticleSystem>(true))
            {
                par.Stop();
            }

            indicator.SetActive(false);

        }
    }

    private void Shoot(Transform target, float minSpread, float maxSpread)
    {

        Vector3 targetPos = target.position;

        // Spawn projectile for each barrel
        List<GameObject> spawnedProjectiles = new List<GameObject>();
        foreach (Transform barrel in shootPoints)
        {
            GameObject o = Instantiate(projectile, barrel.position, projectile.transform.rotation);
            spawnedProjectiles.Add(o);

            GameObject vfx = Instantiate(shootVFXPrefab, barrel);
            vfx.transform.localPosition = Vector3.zero;
        }
        RangeAttack temp = spawnedProjectiles[0].GetComponent<RangeAttack>();

        Vector3 aimRotation;

        // If target in vision, aim at target first
        if (InVision(target))
        {
            // Determine lead strength
            float travelTime = (targetPos - transform.position).magnitude / temp.Speed;
            float strength = Random.Range(leadStrength.x, leadStrength.y);
            Vector3 targetVel = target.GetComponent<Rigidbody>().velocity;
            targetVel.y = Mathf.Clamp(targetVel.y, -10, 10);
            Vector3 leadPos = target.GetComponent<PlayerController>().CenterMass.position
                + (targetVel * travelTime * strength);

            // determined initial rotation
            aimRotation = Quaternion.LookRotation(leadPos - shootPoints[0].position).eulerAngles;
        }
        else
        {
            // If target not in vision, then just aim forwards
            aimRotation = Quaternion.LookRotation(transform.forward).eulerAngles;
        }

        aimRotation = ApplySpread(aimRotation, minSpread, maxSpread);
        
        // Aim shot at target position, activate
        foreach (GameObject shot in spawnedProjectiles)
        {
            shot.transform.rotation = Quaternion.Euler(aimRotation);
            shot.GetComponent<RangeAttack>().Activate();
        }
    }

    private Vector3 ApplySpread(Vector3 rot, float minSpread, float maxSpread)
    {
        float xMod = Random.Range(minSpread, maxSpread);
        float yMod = Random.Range(minSpread, maxSpread);

        // Apply random signs to spread rotaiton
        if (Random.Range(0, 2) == 0)
            xMod *= -1;
        if (Random.Range(0, 2) == 0)
            yMod *= -1;

        // calculate and apply rotation between given range
        rot.x += xMod;
        rot.y += yMod;

        return rot;
    }

    private bool InVision(Transform target)
    {
        Vector3 targetPos = new Vector3(target.position.x, transform.position.y, target.position.z);
        Vector3 temp = (targetPos - transform.position).normalized;
        float angle = Vector3.SignedAngle(temp, transform.forward, Vector3.up);
        return (Mathf.Abs(angle) <= visionRange);
    }
}
