/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - April 7th, 2022
 * Last Edited - Aoril 7th, 2022 by Ben Schuster
 * Description - Damage pool that temporarily exists and targets each team
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePool : MonoBehaviour
{
    [SerializeField] protected float lifeDuration;
    [SerializeField] protected bool affectedByTimestop;
    ScaledTimer lifeTracker;

    [SerializeField] protected TeamDamage initialDamageProfiles;
    [SerializeField] protected TeamDamage tickDamageProfiles;

    [Tooltip("Time between each tick")]
    [SerializeField] private float tickRate;

    private DamageField targetField;

    private void Start()
    {
        lifeTracker = new ScaledTimer(lifeDuration, affectedByTimestop);

        targetField = GetComponentInChildren<DamageField>();
        if(targetField != null )
            targetField.InitValues(initialDamageProfiles, tickDamageProfiles, tickRate);
    }

    private void Update()
    {
        if(lifeTracker.TimerDone())
            Destroy(gameObject);
    }
}
