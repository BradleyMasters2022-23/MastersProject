using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Sirenix.OdinInspector;

public class Explosive : MonoBehaviour
{
    [SerializeField] private List<string> damagableTags;

    [SerializeField] private List<Damagable> damagedTargets;
    [SerializeField] private float explosiveRadius;
    [SerializeField] private float damageDuration = 1;
    [SerializeField] private float VFXDuration = 1;
    [SerializeField] private bool affectedByTimestop = true;
    [SerializeField] private bool solidInTimetop = false;
    private ScaledTimer VFXLifeTracker;
    private ScaledTimer damageLifeTracker;

    private SphereCollider col;
    [SerializeField] private ParticleSystem VFX;
    [SerializeField] private VisualEffect newVFX;
    private float speedMod;

    public int damage;

    private float lastTimeScale;

    public float horizontalForce = 200;
    public float verticalForce = 50;

    [SerializeField] private AnimationCurve knockbackFalloff;

    private void Awake()
    {
        // Prepare references and values
        damagedTargets = new List<Damagable>();
        damageLifeTracker = new ScaledTimer(damageDuration, affectedByTimestop);
        VFXLifeTracker = new ScaledTimer(VFXDuration, affectedByTimestop);
        newVFX.Stop();
        col = GetComponent<SphereCollider>();
        lastTimeScale = TimeManager.WorldTimeScale;
        
        // Modify VFX playback to fit explosive duration
        //ParticleSystem.MainModule main = VFX.main;
        //VFX.Stop();
        //main.playOnAwake = false;
        //speedMod = VFX.main.duration / VFXDuration;
        //main.simulationSpeed = main.simulationSpeed * speedMod;
        //newVFX.playRate = speedMod;
    }


    public void Detonate()
    {
        Detonate(damage);
    }
    public void Detonate(float newDamage)
    {
        col.radius = explosiveRadius;
        col.enabled = true;

        //VFX.Play();
        newVFX.Play();
    }

    private void Update()
    {
        if(VFXLifeTracker.TimerDone() && damageLifeTracker.TimerDone())
        {
            Destroy(gameObject);
        }
        else if(VFXLifeTracker.TimerDone() && VFX.gameObject.activeInHierarchy)
        {
            newVFX.Stop();
            newVFX.gameObject.SetActive(false);
            //VFX.Stop();
            //VFX.gameObject.SetActive(false);
        }
        else if(damageLifeTracker.TimerDone() && col.enabled)
        {
            col.enabled = false;
        }

        VFXTimeSlow();
        if (col.enabled && solidInTimetop)
        {
            ExplosionCol();
        }

        lastTimeScale = TimeManager.WorldTimeScale;
    }

    private void VFXTimeSlow()
    {
        if (lastTimeScale != TimeManager.WorldTimeScale)
        {
            //ParticleSystem.MainModule main = VFX.main;
            //main.simulationSpeed = 1 * speedMod * TimeManager.WorldTimeScale;
            newVFX.playRate = 1 * TimeManager.WorldTimeScale;
        }
    }

    private void ExplosionCol()
    {
        if(TimeManager.WorldTimeScale != 1 && col.isTrigger)
        {
            col.isTrigger = false;
        }
        else if(TimeManager.WorldTimeScale == 1 && !col.isTrigger)
        {
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject rootTgt = other.transform.root.gameObject;
        //Debug.Log("Checking object with of " + rootTgt.name);
        // if target was already damaged or not marked for team, then
        if (!damagableTags.Contains(rootTgt.tag))
            return;

        Damagable target;
        if(rootTgt.TryGetComponent<Damagable>(out target))
        {
            if(damagedTargets.Contains(target))
            {
                //Debug.Log(rootTgt.name + " has already been damaged by this grenade");
                return;
            }
            damagedTargets.Add(target);
            target.Damage(damage);
            target.ExplosiveKnockback(transform.position, transform.localScale.x,
                horizontalForce, verticalForce, explosiveRadius, knockbackFalloff);
        }
    }
}
