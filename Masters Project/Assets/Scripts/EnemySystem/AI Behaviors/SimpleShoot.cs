/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - December 6, 2022
 * Last Edited - December 6, 2022 by Ben Schuster
 * Description - Concrete attack behavior for shooting at a given target
 * ================================================================================================
 */
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Masters.AI;

public class SimpleShoot : AttackTarget
{

    [Header("=== Ranged Attack Data ===")]

    [Tooltip("The projectile prefab to use")]
    public GameObject projectile;
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
    [Tooltip("In a frontal cone, how infront of the enemy does the player need to be to apply lead to shots")]
    [SerializeField] private float leadConeRadius;

    [Space(10)]
    [Tooltip("What is the range of accuracy for the first shot")]
    [SerializeField] private Vector2 firstShotAccuracy;
    [Tooltip("What is the range of accuracy on this")]
    [SerializeField] private Vector2 normalAccuracyRange;

    [Space(10)]
    [Tooltip("Whether the enemy can rotate to the target while attacking")]
    [SerializeField] private bool rotateDuringAttack;
    [Tooltip("In a frontal cone, how infront of the enemy does player need to be to follow")]
    [HideIf("@this.rotateDuringAttack == false")]
    [SerializeField] private float attackTrackConeRange;

    [Tooltip("In a frontal cone, how infront of the enemy does player need to be to follow during indicator")]
    [HideIf("@this.rotateDuringIndication == false")]
    [SerializeField] private float indicatorTrackConeRange;
    [Tooltip("When lost target, how long does it stay looking in a direction before reaquiring player." +
        "Set larger to indicator duration to prevent them from reaquiring completely." +
        "Stun ends early if player reenters the cone.")]
    [HideIf("@this.rotateDuringIndication == false")]
    [SerializeField] private float indicatorStunnedDuration;

    [Header("=== Visuals and Sounds ===")]

    [Tooltip("The object holding the VFX for player indicator")]
    [SerializeField, AssetsOnly] private GameObject indicatorVFXPrefab;
    [SerializeField] private float indicatorScale = 1; 
    [Tooltip("VFX that plays on each shoot point when a projectile is fired")]
    [SerializeField, AssetsOnly] private GameObject shootVFXPrefab;
    [SerializeField] private float shootVFXScale = 1;

    [SerializeField] private AudioClip indicatorSFX;
    [SerializeField] private AudioClip shootSFX;

    /// <summary>
    /// Get the indicator gameobject references
    /// </summary>
    private GameObject[] indicators;

    /// <summary>
    /// Internal tracker for the time between each shot
    /// </summary>
    private ScaledTimer shotTimer;
    /// <summary>
    /// Internal tracker for the time enemy is stunned during indicator phase
    /// </summary>
    private ScaledTimer indicatorStunTracker;

    /// <summary>
    /// whether the enemy is currently stunned in indicator state
    /// </summary>
    [SerializeField] private bool indicatorStunned = false;

    private float averageLead;

    /// <summary>
    /// Initialize each thing
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        // Spawn in indicator and set scale, but then disable until use
        if(indicatorVFXPrefab!= null )
        {
            indicators = new GameObject[shootPoints.Length];
            for (int i = 0; i < shootPoints.Length; i++)
            {
                indicators[i] = Instantiate(indicatorVFXPrefab, shootPoints[i]);
                indicators[i].transform.localScale *= indicatorScale;

                for(int j = 0; j < indicators[i].transform.childCount; j++)
                {
                    indicators[i].transform.GetChild(j).localScale *= indicatorScale;
                }

                indicators[i].transform.localPosition = Vector3.zero;
                indicators[i].SetActive(false);
            }
        }

        indicatorStunTracker = new ScaledTimer(indicatorStunnedDuration);
        averageLead = (leadStrength.x + leadStrength.y) / 2;
        attackReady = true;
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

            // dont go into cooldown after last shot
            if (i == numOfShots - 1)
                break;

            int c = 0;
            shotTimer.ResetTimer();
            while(!shotTimer.TimerDone())
            {
                c++;
                if(c >= 10000)
                {
                    Debug.Log("Damage action shot timer stuck in infinite loop");
                    yield break;
                }

                yield return null;
            }
        }
    }

    protected override void IndicatorUpdateFunctionality()
    {
        // This only tracks rotation. So if don't rotate, just exit
        if(!rotateDuringIndication)
        {
            return;
        }

        // if not alreadys stunned...
        if(!indicatorStunned)
        {
            // If in vision, track
            if(transform.InVisionCone(target, indicatorTrackConeRange))
            {
                Vector3 leadPos = GetLeadedPosition(target, averageLead);
                // reduce lead on Y for just looking
                leadPos.y = target.position.y;
                RotateToTarget(leadPos);
            }
            // Otherwise, begin stun
            else
            {
                indicatorStunned = true;
                indicatorStunTracker.ResetTimer();
            }
        }
        // If already stunned...
        else
        {
            // If in vision, return to tracking
            if (transform.InVisionCone(target, indicatorTrackConeRange)
                || indicatorStunTracker.TimerDone())
            {
                indicatorStunned = false;
            }
        }
    }

    protected override void DamageUpdateFunctionality()
    {
        // If the target is in vision, keep rotating. Otherwise, stop
        if(rotateDuringAttack && 
            transform.InVisionCone(target, attackTrackConeRange))
        {
            Vector3 leadPos = GetLeadedPosition(target, averageLead);
            RotateToTarget(leadPos);
        }
    }

    protected override void ShowIndicator()
    {
        if (indicatorVFXPrefab == null)
        {
            return;
        }

        if (indicatorSFX != null)
        {
            source.Stop();
            source.PlayOneShot(indicatorSFX);
        }
            

        // Tell each one to start
        foreach (GameObject indicator in indicators)
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
        if (indicatorVFXPrefab == null)
        {
            return;
        }

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

            if(shootVFXPrefab!= null)
            {
                GameObject vfx = Instantiate(shootVFXPrefab, barrel);
                vfx.transform.localPosition = Vector3.zero;
                vfx.transform.localScale *= shootVFXScale;
            }
        }
        
        Vector3 aimRotation;

        // If target in vision, aim at target first
        if (transform.InVisionCone(target, leadConeRadius))
        {
            //RangeAttack temp = spawnedProjectiles[0].GetComponent<RangeAttack>();

            //// Determine lead strength
            //float travelTime = (targetPos - transform.position).magnitude / temp.Speed;
            //float strength = Random.Range(leadStrength.x, leadStrength.y);
            //Vector3 targetVel = target.GetComponent<Rigidbody>().velocity;

            //targetVel.y = 0;

            //Vector3 leadPos = target.GetComponent<PlayerController>().CenterMass.position
            //    + (targetVel * travelTime * strength);

            float strength = Random.Range(leadStrength.x, leadStrength.y);
            Vector3 leadPos = GetLeadedPosition(target.GetComponent<PlayerController>().CenterMass, strength);

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
        
        if(shootSFX != null)
        {
            source.Stop();
            source.clip = shootSFX;
            source.Play();
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

    private Vector3 GetLeadedPosition(Transform target, float strength)
    {
        if (strength == 0)
        {
            // Debug.Log("No lead strenght, setting to target position at: " + target.position);
            return target.position;
        }
            

        RangeAttack temp = projectile.GetComponent<RangeAttack>();

        // Determine lead strength. not perfect but works for now
        float travelTime = (target.position - transform.position).magnitude / (temp.Speed);
        Vector3 targetVel = target.root.GetComponent<Rigidbody>().velocity;

        targetVel.y = 0;

        Vector3 leadPos = target.position + (targetVel * travelTime * strength);

        return leadPos;
    }
}
