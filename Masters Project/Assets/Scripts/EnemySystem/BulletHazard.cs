using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHazard : TimeAffectedEntity
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private float fireDelay;
    [SerializeField] private float initialDelay;
    [SerializeField] Transform shootPoint;
    private LocalTimer fireTimer;

    [SerializeField] AudioClipSO shootSFX;
    [SerializeField] GameObject shootVFX;

    [SerializeField] private AudioSource source;

    private void Start()
    {
        fireTimer = GetTimer(initialDelay);

        StartCoroutine(EndlessShoot());
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
                proj.GetComponent<RangeAttack>().Activate();
            }
            
            // Reset fire delay
            fireTimer.ResetTimer(fireDelay);
            yield return waitTick;
            yield return null;
        }
    }
}
