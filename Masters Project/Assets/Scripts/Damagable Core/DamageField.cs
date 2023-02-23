using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DamageField : MonoBehaviour
{
    public float initialDamage;
    public float tickDamage;

    public float playerInitialDamage;
    public float playerTickDamage;

    public float tickRate;

    private Dictionary<Target, ScaledTimer> targetEntities;
    private Collider coll;

    private void Awake()
    {
        targetEntities = new Dictionary<Target, ScaledTimer>();
        coll = GetComponent<Collider>();
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

                    if (entity.Key.Team == Team.ENEMY)
                    {
                        entity.Key.RegisterEffect(tickDamage);
                    }
                    // If player, make it take tick damage in timestop
                    else if (entity.Key.Team == Team.PLAYER)
                    {
                        entity.Key.RegisterEffect(playerTickDamage);
                        Debug.Log("Registering player damage tick");
                    }
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
                // If an enemy, make it not take tick damage in timestop
                if(newTar.Team == Team.ENEMY)
                {
                    targetEntities.Add(newTar, new ScaledTimer(tickRate, true));
                    newTar.RegisterEffect(initialDamage);
                }
                // If player, make it take tick damage in timestop
                else if(newTar.Team == Team.PLAYER)
                {
                    targetEntities.Add(newTar, new ScaledTimer(tickRate, false));
                    newTar.RegisterEffect(playerInitialDamage);
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
