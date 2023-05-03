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
    [SerializeField] protected float vanishTime;
    [SerializeField] protected bool affectedByTimestop;
    ScaledTimer lifeTracker;
    bool vanishing = false;

    [SerializeField] protected TeamDamage initialDamageProfiles;
    [SerializeField] protected TeamDamage tickDamageProfiles;

    [Tooltip("Time between each tick")]
    [SerializeField] private float tickRate;

    [SerializeField] private AudioClipSO ambientSFX;
    private DamageField targetField;
    private AudioSource source;

    private void Start()
    {
        lifeTracker = new ScaledTimer(lifeDuration, affectedByTimestop);

        targetField = GetComponentInChildren<DamageField>();
        source = GetComponent<AudioSource>();

        ambientSFX.PlayClip(source);

        if(targetField != null )
            targetField.InitValues(initialDamageProfiles, tickDamageProfiles, tickRate);
    }

    private void Update()
    {
        if(!vanishing && lifeTracker.TimerDone())
        {
            vanishing = true;
            StartCoroutine(Vanish());
        }
    }

    private IEnumerator Vanish()
    {
        // get orignal position and position below current target
        Vector3 originalPos = transform.position;
        Vector3 tarPos = transform.position + transform.forward * -2;

        // lerp to target position over vanish time
        lifeTracker.ResetTimer(vanishTime);
        while (!lifeTracker.TimerDone())
        {
            transform.position = Vector3.Lerp(originalPos, tarPos, lifeTracker.TimerProgress());
            yield return null;
        }

        // destroy when over
        Destroy(gameObject);
    }
}
