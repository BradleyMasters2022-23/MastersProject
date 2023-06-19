using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHazard : TimeAffectedEntity
{
    [Header("Projectile Stats")]

    [SerializeField] private GameObject projectile;
    [SerializeField] Transform shootPoint;
    [SerializeField] AudioClipSO shootSFX;
    [SerializeField] GameObject shootVFX;
    [SerializeField] float speedModifier = 1;
    [SerializeField] float damageMultiplier = 1;
    [SerializeField] float range = 100f;

    [Header("Trap Stats")]

    [SerializeField] bool playOnStart = false;
    [SerializeField] private AudioSource source;
    [SerializeField] private float initialDelay;
    [SerializeField] private float fireDelay;

    private LocalTimer fireTimer;

    private void Start()
    {
        if (playOnStart)
            Play();
    }

    /// <summary>
    /// repeatedly fire bullets
    /// </summary>
    /// <returns></returns>
    private IEnumerator EndlessShoot()
    {
        // wait for the initial tick
        WaitUntil waitTick = new WaitUntil(fireTimer.TimerDone);
        yield return waitTick;

        while (true)
        {
            // Get VFX to shoot
            GameObject vfx = VFXPooler.instance.GetVFX(shootVFX);
            if(vfx == null)
                vfx = Instantiate(shootVFX, shootPoint.position, shootPoint.rotation);
            if (vfx != null)
            {
                vfx.transform.position = shootPoint.position;
                vfx.transform.rotation = shootPoint.rotation;
            }

            shootSFX.PlayClip(source);

            // Get bullets 
            GameObject proj = ProjectilePooler.instance.GetProjectile(projectile);
            if(proj == null)
                proj = Instantiate(projectile, shootPoint.position, shootPoint.rotation);
            if (proj != null)
            {
                proj.transform.position = shootPoint.position;
                proj.transform.rotation = shootPoint.rotation;
                proj.GetComponent<RangeAttack>().Initialize(damageMultiplier, speedModifier, range, gameObject);
            }
            
            // Reset fire delay
            fireTimer.ResetTimer(fireDelay);
            yield return waitTick;
            yield return null;
        }
    }
    public void Play()
    {
        fireTimer = GetTimer(initialDelay);

        StartCoroutine(EndlessShoot());
    }
    public void Stop()
    {
        StopAllCoroutines();
    }
}
