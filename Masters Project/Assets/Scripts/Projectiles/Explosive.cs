using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Explosive : MonoBehaviour
{
    [SerializeField] private List<string> damagableTags;

    [SerializeField] private List<GameObject> affectedEnemies;
    [SerializeField] private float explosiveRadius;
    [SerializeField, Range(0.01f, 999)] private float duration;
    [SerializeField] private bool affectedByTimestop;
    private ScaledTimer lifeTracker;

    private SphereCollider col;
    [SerializeField] private ParticleSystem VFX;
    private float speedMod;

    public int damage;

    private float lastTimeScale;

    public float horizontalForce;
    public float verticalForce;

    private void Awake()
    {
        // Prepare references and values
        affectedEnemies = new List<GameObject>();
        lifeTracker = new ScaledTimer(duration, affectedByTimestop);
        col = GetComponent<SphereCollider>();
        lastTimeScale = TimeManager.WorldTimeScale;

        // Modify VFX playback to fit explosive duration
        ParticleSystem.MainModule main = VFX.main;
        VFX.Stop();
        main.playOnAwake = false;
        speedMod = VFX.main.duration / duration;
        main.simulationSpeed = main.simulationSpeed * speedMod;
    }

    private void Start()
    {
        Detonate();
    }


    public void Detonate()
    {
        Detonate(damage);
    }
    public void Detonate(float newDamage)
    {
        col.radius = explosiveRadius;
        col.enabled = true;

        VFX.Play();
    }

    private void Update()
    {
        if(lifeTracker.TimerDone())
        {
            Destroy(gameObject);
        }

        VFXTimeSlow();
        lastTimeScale = TimeManager.WorldTimeScale;
    }

    private void VFXTimeSlow()
    {
        if (lastTimeScale != TimeManager.WorldTimeScale)
        {
            ParticleSystem.MainModule main = VFX.main;
            main.simulationSpeed = 1 * speedMod * TimeManager.WorldTimeScale;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // if target was already damaged or not marked for team, then
        if (affectedEnemies.Contains(other.gameObject)
            && !damagableTags.Contains(other.tag))
            return;

        Damagable target;
        if(other.TryGetComponent<Damagable>(out target))
        {
            //affectedEnemies.Add(other.gameObject);
            Debug.Log("Damaging the bastard of " + other.gameObject.name);
            target.Damage(damage);
            //target.ApplyKnockback(transform.position, 100, false);
            target.NewKnockback(transform.position, horizontalForce, verticalForce, explosiveRadius);
        }
    }

}
