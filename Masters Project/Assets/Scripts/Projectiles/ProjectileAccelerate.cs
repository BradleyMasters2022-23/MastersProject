/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 24th, 2022
 * Last Edited - October 24th, 2022 by Ben Schuster
 * Description - Concrete projectile that flies straight
 * ================================================================================================
 */
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class ProjectileAccelerate : Projectile
{
    [Header("--- Acceleration Stuff ---")]
 
    [SerializeField] private float acceleratingRate;
    [SerializeField] private float targetEndVelocity;

    private float flyTime;
    private bool reachedTarget;

    [ShowIf("@this.acceleratingRate < 1")]
    [Tooltip("If this shot is decelerating and can stop, how long does it live for?")]
    [SerializeField] private float projectileLifetime;

    private ScaledTimer lifetime;

    // Update is called once per frame
    protected override void Fly()
    {
        Accelerate();
        base.Fly();
    }

    protected override void CheckLife()
    {
        base.CheckLife();
        
        if (lifetime != null && lifetime.TimerDone())
            End();
    }

    /// <summary>
    /// Activate this projectile and allow it to fire
    /// </summary>
    public override void Activate()
    {
        if(acceleratingRate < 1)
            lifetime = new ScaledTimer(projectileLifetime, affectedByTimestop);

        base.Activate();
    }

    private void Accelerate()
    {
        if (!reachedTarget)
        {
            if (affectedByTimestop)
                flyTime += TimeManager.WorldDeltaTime;
            else
                flyTime += Time.deltaTime;


            targetVelocity = transform.forward * speed * Mathf.Pow(acceleratingRate, flyTime);
        }
        else if (targetVelocity.magnitude != targetEndVelocity)
        {
            targetVelocity = transform.forward * targetEndVelocity;
        }

        //rb.velocity = targetVelocity * TimeManager.WorldTimeScale;


        // if accelerating, check to see if it reached its max velocity
        if (!reachedTarget
            && ((acceleratingRate > 1 && targetVelocity.magnitude >= targetEndVelocity)
            || (acceleratingRate < 1 && targetVelocity.magnitude <= targetEndVelocity)))
        {
            reachedTarget = true;
        }

        //futurePos = transform.position + rb.velocity;
    }
}
