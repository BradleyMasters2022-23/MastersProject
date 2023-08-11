/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - February 22th, 2022
 * Last Edited - February 22th, 2022 by Ben Schuster
 * Description - Damage field that deals damage over time
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DamageField : MonoBehaviour
{
    [SerializeField] protected TeamDamage initialDamageProfiles;
    [SerializeField] protected TeamDamage tickDamageProfiles;

    //[Tooltip("Initial damage dealt to enemies when first entering trap")]
    //[SerializeField] private float initialDamage;
    //[Tooltip("Continuous damage dealt to enemies on each tick")]
    //[SerializeField] private float tickDamage;

    //[Tooltip("Initial damage dealt to player when first entering the trap")]
    //[SerializeField] private float playerInitialDamage;
    //[Tooltip("Continuous damage dealt to player on each tick")]
    //[SerializeField] private float playerTickDamage;

    [Tooltip("Time between each tick")]
    [SerializeField] private float tickRate;

    private Dictionary<Target, ScaledTimer> targetEntities;
    private Collider coll;

    private void Awake()
    {
        targetEntities = new Dictionary<Target, ScaledTimer>();
        coll = GetComponent<Collider>();
    }

    public void InitValues(TeamDamage initDmgProf, TeamDamage tickDmgProf, float tkR)
    {
        initialDamageProfiles = initDmgProf;
        tickDamageProfiles = tickDmgProf;
        tickRate = tkR;
    }

    private void LateUpdate()
    {
        // reset list if collider was disabled
        if (coll.enabled == false && targetEntities.Count > 0)
            ResetEntities();

        if(targetEntities != null)
        {
            // Check each entity in list, try dealing damage
            foreach(var entity in targetEntities.ToList())
            {
                // Check if entity even exists, remove if not
                if(entity.Key == null)
                    targetEntities.Remove(entity.Key);

                // Check if its tick timer is done, and deal damage if so
                if(entity.Value.TimerDone())
                {
                    entity.Value.ResetTimer();

                    entity.Key.RegisterEffect(tickDamageProfiles, transform.position);
                    entity.Key.Knockback(tickDamageProfiles, transform.position);
                }
            }

        }
    }

    public void ResetEntities()
    {
        if(targetEntities != null)
            targetEntities.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        Target newTar;

        if(other.transform.root.TryGetComponent<Target>(out newTar))
        {
            // stop if already registered
            if(targetEntities.ContainsKey(newTar))
            {
                return;
            }

            // If not already registered...
            else
            {
                newTar.RegisterEffect(initialDamageProfiles, transform.position);
                newTar.Knockback(initialDamageProfiles, transform.position);

                // If an enemy, make it not take tick damage in timestop
                if (newTar.Team == Team.ENEMY)
                {
                    targetEntities.Add(newTar, new ScaledTimer(tickRate, true));
                }
                // If player, make it take tick damage in timestop
                else if(newTar.Team == Team.PLAYER)
                {
                    targetEntities.Add(newTar, new ScaledTimer(tickRate, false));
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Target newTar;
        if (other.transform.root.TryGetComponent<Target>(out newTar))
        {
            // Remove target from trigger
            if (targetEntities.ContainsKey(newTar))
            {
                targetEntities.Remove(newTar);
            }
        }
    }
}
