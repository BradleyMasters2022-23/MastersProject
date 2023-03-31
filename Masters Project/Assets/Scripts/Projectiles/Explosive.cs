using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Sirenix.OdinInspector;

public class Explosive : MonoBehaviour
{
    [SerializeField] private List<string> damagableTags;

    [SerializeField] private List<Target> damagedTargets;
    [SerializeField] private float explosiveRadius;
    [SerializeField] private float damageDuration = 1;
    [SerializeField] private float VFXDuration = 1;
    [SerializeField] private bool affectedByTimestop = true;
    [SerializeField] private bool solidInTimetop = false;
    private ScaledTimer VFXLifeTracker;
    private ScaledTimer damageLifeTracker;

    private SphereCollider col;
    [SerializeField] private VisualEffect VFX;
    private float speedMod;

    public int damage;
    public int playerDamage;

    private float lastTimeScale;

    public float horizontalForce = 200;
    public float verticalForce = 50;

    [SerializeField] private AnimationCurve knockbackFalloff;

    [SerializeField] private bool instantDetoante = false;

    [SerializeField] private AudioClipSO explosionSound;

    [SerializeField] private bool requireLoS;
    [SerializeField] private LayerMask lineOfSightLayers;

    private void Awake()
    {
        // Prepare references and values
        damagedTargets = new List<Target>();
        damageLifeTracker = new ScaledTimer(damageDuration, affectedByTimestop);
        VFXLifeTracker = new ScaledTimer(VFXDuration, affectedByTimestop);
        VFX.Stop();
        col = GetComponent<SphereCollider>();
        lastTimeScale = TimeManager.WorldTimeScale;

        if (instantDetoante)
            Detonate();
    }


    public void Detonate()
    {
        //Debug.Log("NEW GRENADE DETONATING");
        Detonate(damage);
    }
    public void Detonate(float newDamage)
    {
        explosionSound.PlayClip(transform);

        col.radius = explosiveRadius;
        col.enabled = true;

        VFX.Play();
    }

    private void Update()
    {
        if(VFXLifeTracker.TimerDone() && damageLifeTracker.TimerDone())
        {
            Destroy(gameObject);
        }
        else if(VFXLifeTracker.TimerDone() && VFX.gameObject.activeInHierarchy)
        {
            VFX.Stop();
            VFX.gameObject.SetActive(false);
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
            VFX.playRate = 1 * TimeManager.WorldTimeScale;
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
        //Debug.Log("Checking collision with :" + other.name);
        Transform parent = other.transform;
        Target target;


        // continually escelate up for a targetable reference
        while (!parent.TryGetComponent<Target>(out target) && parent.parent != null)
        {
            parent = parent.parent;
        }
        //Debug.Log("parent landed on " + parent.name);


        // if target was already damaged or not marked for team, then
        if (target != null && damagableTags.Contains(target.tag) && !damagedTargets.Contains(target))
        {
            //Debug.Log($"{target.name} not damaged, adding to list");
            damagedTargets.Add(target);


            //// Test for LOS first
            //if(requireLoS)
            //{
            //    Ray testRay = new Ray(transform.position, target.transform.position - transform.position);
                
            //    RaycastHit col;
            //    if(Physics.Raycast(testRay, out col, Mathf.Infinity, lineOfSightLayers))
            //    {
            //        Debug.DrawLine(transform.position, transform.position + testRay.direction * col.distance, Color.red, 1f);

            //        Target hitTar = col.rigidbody.GetComponent<Target>();
            //        if (hitTar == null || hitTar != target)
            //            return;
            //    }
            //}

            if (target.CompareTag("Player"))
                target.RegisterEffect(playerDamage);
            else
                target.RegisterEffect(damage);

        }
    }
}
