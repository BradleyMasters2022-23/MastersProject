/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 23th, 2022
 * Last Edited - October 23th, 2022 by Ben Schuster
 * Description - Manage the shooting for the player
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGunController : MonoBehaviour
{
    [Header("---Game Flow---")]
    [SerializeField] private ChannelGMStates onStateChangeChannel;

    [Header("=====Gameplay=====")]

    [Tooltip("Damage of the bullets this gun fires")]
    [SerializeField] private UpgradableFloat damageMultiplier;
    [Tooltip("Speed of the bullets this gun fires")]
    [SerializeField] private UpgradableFloat speedMultiplier;
    [Tooltip("Seconds takes between each shot")]
    [SerializeField] private UpgradableFloat fireDelay;
    [Tooltip("Projectile thats fired from this gun")]
    [SerializeField] private GameObject shotPrefab;
    [Tooltip("How many projectiles are actually fired when shot")]
    [SerializeField] private int bulletsPerShot;
    [Tooltip("VFX for firing the gun")]
    [SerializeField] private GameObject muzzleflashVFX;
    [Tooltip("Minimum range for aiming to take effect. " +
        "Prevents weird aiming when too close to a wall")]
    [SerializeField] private float minAimRange;

    [Tooltip("Sound that happens when gun go pew pew")]
    [SerializeField] private AudioClip[] gunshotSound;
    private AudioSource source;

    [Header("=====Weapon Bloom [Accuracy]=====")]
    [Tooltip("What is the default/target bloom. 0 for perfect accuracy")]
    [SerializeField] private float baseBloom;
    [Tooltip("Amount of bloom (inaccuracy) gained per shot. Based on the shot, not bullets")]
    [SerializeField] private float bloomPerShot;
    [Tooltip("Maximum amount of bloom (inaccuracy) allowed.")]
    [SerializeField] private float maxBloom;
    [Tooltip("Speed at which bloom decreases back to base bloom when not firing")]
    [SerializeField] private float bloomRecoveryRate;
    /// <summary>
    /// Current amount of bloom currently active
    /// </summary>
    private float currBloom;
    [Tooltip("When a bullet is spawned, how much displacement from its origin can it have.")]
    [SerializeField] private float maxShootDisplacement;

    [Header("=====Setup=====")]

    [Tooltip("Where the shot fires from")]
    [SerializeField] private Transform shootPoint;
    [Tooltip("Default target")]
    [SerializeField] private Transform defaultTarget;
    [Tooltip("Layers for raycast to ignore")]
    [SerializeField] LayerMask layersToIgnore;


    [Header("Enhanced Bullets")]
    [SerializeField] private GameObject enhancedBulletPrefab;
    [SerializeField] private float enhancedSpeedMultiplier;

    /// <summary>
    /// Whether this gun is shooting
    /// </summary>
    private bool firing;
    /// <summary>
    /// Main camera that handles the targeting
    /// </summary>
    private CameraShoot shootCam;
    /// <summary>
    /// Core controller mapping
    /// </summary>
    private GameControls controller;
    /// <summary>
    /// Input for shooting
    /// </summary>
    private InputAction shoot;
    /// <summary>
    /// Tracker for between shots
    /// </summary>
    private ScaledTimer fireTimer;

    /// <summary>
    /// Initialize controls and starting values
    /// </summary>
    private void Awake()
    {
        // Initialize controls
        StartCoroutine(InitializeControls());

        // Initialize upgradable variables
        damageMultiplier.Initialize();
        speedMultiplier.Initialize();
        fireDelay.Initialize();

        // Initialize timers
        fireTimer = new ScaledTimer(fireDelay.Current, false);

        source = GetComponent<AudioSource>();
    }

    private IEnumerator InitializeControls()
    {
        while(GameManager.controls == null)
            yield return null;

        controller = GameManager.controls;
        shoot = controller.PlayerGameplay.Shoot;
        shoot.Enable();
        shoot.started += ToggleTrigger;
        shoot.canceled += ToggleTrigger;

        yield return null;
    }

    /// <summary>
    /// Get outside references, initialize
    /// </summary>
    private void Start()
    {
        // Get reference to camera shoot, initialize
        shootCam = Camera.main.GetComponent<CameraShoot>();
        shootCam.Initialize(defaultTarget, layersToIgnore, minAimRange);
    }

    private void FixedUpdate()
    {
        // Tell the accuracy to recover over time while not firing
        if(!firing && currBloom != baseBloom)
        {
            float newAccuracy = currBloom - bloomRecoveryRate * Time.deltaTime;
            if(newAccuracy < baseBloom)
            {
                newAccuracy = baseBloom;
            }
                

            currBloom=newAccuracy;
        }
    }

    /// <summary>
    /// Toggle firing based on input
    /// </summary>
    /// <param name="ctx">Callback context on input</param>
    private void ToggleTrigger(InputAction.CallbackContext ctx)
    {
        firing = ctx.started;
    }

    /// <summary>
    /// Fire the projectile
    /// </summary>
    private void Shoot()
    {
        if(muzzleflashVFX != null)
        {
            Instantiate(muzzleflashVFX, shootPoint.position, shootPoint.transform.rotation);
        }

        for(int i = 0; i < bulletsPerShot; i++)
        {
            // Shoot projectile, aiming towards passed in target
            GameObject newShot;

            if(TimeManager.WorldTimeScale == 1)
            {
                newShot=Instantiate(shotPrefab, shootPoint.position, transform.rotation);
            }
            else
            {
                newShot=Instantiate(enhancedBulletPrefab, shootPoint.position, transform.rotation);
            }

            // Calculate & apply the new minor displacement
            Vector3 displacement = new Vector3(
                Random.Range(-maxShootDisplacement, maxShootDisplacement),
                Random.Range(-maxShootDisplacement, maxShootDisplacement),
                Random.Range(-maxShootDisplacement, maxShootDisplacement));
            newShot.transform.position += displacement;

            // Aim to center screen, apply inaccuracy bonuses
            newShot.transform.LookAt(shootCam.TargetPos);
            if (TimeManager.WorldTimeScale == 1)
            {
                newShot.transform.eulerAngles = ApplySpread(newShot.transform.eulerAngles);
                newShot.GetComponent<RangeAttack>().Initialize(damageMultiplier.Current, speedMultiplier.Current, true);
            }
            else
            {
                newShot.GetComponent<RangeAttack>().Initialize(damageMultiplier.Current, speedMultiplier.Current * enhancedSpeedMultiplier, true);
            }
            

            // Tell bullet to initialize
            
        }

        if(gunshotSound.Length > 0)
            source.PlayOneShot(gunshotSound[Random.Range(0, gunshotSound.Length)],0.3f);

        // Increase bloom after shot
        currBloom = Mathf.Clamp(currBloom + bloomPerShot, baseBloom, maxBloom);
    }

    private void EnhancedShoot()
    {
        if (muzzleflashVFX != null)
        {
            Instantiate(muzzleflashVFX, shootPoint.position, shootPoint.transform.rotation);
        }

        GameObject newShot = Instantiate(enhancedBulletPrefab, shootPoint.position, transform.rotation);

        // Calculate & apply the new minor displacement
        Vector3 displacement = new Vector3(
            Random.Range(-maxShootDisplacement, maxShootDisplacement),
            Random.Range(-maxShootDisplacement, maxShootDisplacement),
            Random.Range(-maxShootDisplacement, maxShootDisplacement));
        newShot.transform.position += displacement;

        // Aim to center screen, apply inaccuracy bonuses
        newShot.transform.LookAt(shootCam.TargetPos);


    }

    private Vector3 ApplySpread(Vector3 rot)
    {
        float xMod = Random.Range(-currBloom, currBloom);
        float yMod = Random.Range(-currBloom, currBloom);

        // calculate and apply rotation between given range
        rot.x += xMod;
        rot.y += yMod;

        return rot;
    }

    /// <summary>
    /// Replace the players shot projectile
    /// </summary>
    /// <param name="p">projectile prefab to fire</param>
    public void LoadNewProjectile(GameObject p)
    {
        shotPrefab = p;
    }

    private void Update()
    {
        // If input to fire and timer is done, shoot
        if(firing && fireTimer.TimerDone())
        {
            Shoot();
            fireTimer.ResetTimer();
        }
    }

    /// <summary>
    /// Disable input to prevent crashing
    /// </summary>
    private void OnDisable()
    {
        shoot.started -= ToggleTrigger;
        shoot.canceled -= ToggleTrigger;
    }

    public float GetDamageMultiplier() {
        return damageMultiplier.Current;
    }

    public void SetDamageMultiplier(float newVal) {
        damageMultiplier.ChangeVal(newVal);
    }

    public float GetBaseDamage()
    {
        return shotPrefab.GetComponent<Projectile>().GetDamage();
    }
}
