/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 23th, 2022
 * Last Edited - March 30th, 2022 by Ben Schuster
 * Description - Manage the shooting for the player
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerGunController : TimeAffectedEntity, TimeObserver
{
    [Header("---Game Flow---")]
    [SerializeField] private ChannelGMStates onStateChangeChannel;
    [SerializeField] private ChannelVoid onWeaponShoot;

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
    [Tooltip("The range of this weapon. Passed through to the bullet")]
    [SerializeField] private float maxRange;

    [Tooltip("Sound that happens when gun go pew pew")]
    [SerializeField] private AudioClipSO gunshotSound;
    [SerializeField] private AudioSource gunshotSource;

    [Header("=====Weapon Bloom [Accuracy]=====")]

    [Tooltip("What is the default/target bloom. 0 for perfect accuracy")]
    [SerializeField] private float baseBloom;
    [Tooltip("Amount of bloom (inaccuracy) gained per shot. Based on the shot, not bullets")]
    [SerializeField] private float bloomPerShot;
    [Tooltip("Maximum amount of bloom (inaccuracy) allowed.")]
    [SerializeField] private float maxBloom;
    [Tooltip("Speed at which bloom decreases back to base bloom when not firing")]
    [SerializeField] private float bloomRecoveryRate;
    [Tooltip("How long after firing to wait before beginning bloom recovery")]
    [SerializeField] private float bloomRecoveryDelay;

    /// <summary>
    /// Current cooldown before recovering bloom
    /// </summary>
    private ScaledTimer bloomRecoveryCDTracker;

    /// <summary>
    /// Current amount of bloom currently active
    /// </summary>
    private float currBloom;
    public float CurrBloom
    {
        get { return currBloom; }
    }
    public float BaseBloom
    {
        get { return baseBloom; }
    }
    public float MaxBloom
    {
        get { return maxBloom; }
    }
    [Tooltip("When a bullet is spawned, how much displacement from its origin can it have.")]
    [SerializeField] private float maxShootDisplacement;

    [Header("=====Setup=====")]

    [Tooltip("Where the shot fires from")]
    [SerializeField] private Transform shootPoint;
    [Tooltip("Default target")]
    [SerializeField] private Transform defaultTarget;
    [Tooltip("Layers for raycast to ignore")]
    [SerializeField] LayerMask hitLayers;

    [SerializeField] private Animator animator;

    [Header("Enhanced Bullets")]
    [SerializeField] private int enhancedBulletsPerShot;
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
        bloomRecoveryCDTracker = new ScaledTimer(bloomRecoveryDelay, false);
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
        if(shootCam == null)
        {
            shootCam = Camera.main.GetComponent<CameraShoot>();
            shootCam.Initialize(defaultTarget, hitLayers, minAimRange);
        }
        
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        // Tell the accuracy to recover over time while not firing
        if(!firing && currBloom != baseBloom && bloomRecoveryCDTracker.TimerDone() && !TimeManager.TimeStopped)
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

        if (ctx.canceled)
            bloomRecoveryCDTracker.ResetTimer();
    }

    /// <summary>
    /// Fire the projectile
    /// </summary>
    private void Shoot()
    {
        if(muzzleflashVFX != null)
        {
            GameObject mf = VFXPooler.instance.GetVFX(muzzleflashVFX);
            if(mf != null)
            {
                mf.transform.position = shootPoint.position;
                mf.transform.rotation = shootPoint.rotation;
            }
            else
            {
                mf = Instantiate(muzzleflashVFX, shootPoint.position, shootPoint.rotation);
            }

            if(Slowed)
            {
                mf.transform.GetChild(0).gameObject.layer = 0;
                
                
            }
            else
            {
                mf.transform.parent = shootPoint;
                mf.transform.GetChild(0).gameObject.layer = 6;
            }
        }

        if(animator != null)
        {
            animator.SetTrigger("Fire");
        }

        // If time is not stopped, fire normal bullets
        if(!Slowed)
        {
            for (int i = 0; i < bulletsPerShot; i++)
            {
                // Shoot projectile, aiming towards passed in target
                GameObject newShot;

                // Try to get pool. If not, just spawn normally
                if(ProjectilePooler.instance != null && ProjectilePooler.instance.HasPool(shotPrefab))
                {
                    newShot = ProjectilePooler.instance.GetProjectile(shotPrefab);
                    newShot.transform.position = shootPoint.position;
                    newShot.transform.rotation = shootPoint.rotation;
                }
                else
                {
                    newShot = Instantiate(shotPrefab, shootPoint.position, shootPoint.rotation);
                }
                

                // Calculate & apply the new minor displacement
                Vector3 displacement = new Vector3(
                    Random.Range(-maxShootDisplacement, maxShootDisplacement),
                    Random.Range(-maxShootDisplacement, maxShootDisplacement),
                    Random.Range(-maxShootDisplacement, maxShootDisplacement));
                newShot.transform.position += displacement;

                // Aim to center screen, apply inaccuracy bonuses
                newShot.transform.LookAt(shootCam.TargetPos);
                newShot.transform.eulerAngles = ApplySpread(newShot.transform.eulerAngles);

                newShot.GetComponent<RangeAttack>().Initialize(damageMultiplier.Current, speedMultiplier.Current, maxRange, gameObject, true);
                currBloom = Mathf.Clamp(currBloom + bloomPerShot, baseBloom, maxBloom);
            }
        }
        // Else if time is stopped, fire enhanced bullets
        else
        {
            for (int i = 0; i < enhancedBulletsPerShot; i++)
            {
                // Shoot projectile, aiming towards passed in target
                GameObject newShot;

                if (ProjectilePooler.instance != null && ProjectilePooler.instance.HasPool(enhancedBulletPrefab))
                {
                    newShot = ProjectilePooler.instance.GetProjectile(enhancedBulletPrefab);
                    newShot.transform.position = shootPoint.position;
                    newShot.transform.rotation = shootPoint.rotation;
                }
                else
                {
                    newShot = Instantiate(enhancedBulletPrefab, shootPoint.position, shootPoint.rotation);
                }

                // Calculate & apply the new minor displacement
                Vector3 displacement = new Vector3(
                    Random.Range(-maxShootDisplacement, maxShootDisplacement),
                    Random.Range(-maxShootDisplacement, maxShootDisplacement),
                    Random.Range(-maxShootDisplacement, maxShootDisplacement));
                newShot.transform.position += displacement;

                // Aim to center screen, apply inaccuracy bonuses
                newShot.transform.LookAt(shootCam.TargetPos);
                newShot.GetComponent<RangeAttack>().Initialize(damageMultiplier.Current, speedMultiplier.Current * enhancedSpeedMultiplier, maxRange, gameObject, true);

            }
        }

        // play sound effect
        gunshotSound.PlayClip(shootPoint, gunshotSource, true);
        // call any subscribers to this event
        onWeaponShoot?.RaiseEvent();
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

    public bool InRange()
    {
        // Debug.Log("SP : " + (shootPoint != null) + " | SC : " + (shootCam != null));
        float dist = Mathf.Abs(Vector3.Distance(shootPoint.position, shootCam.TargetPos)-0.5f);
        return dist < maxRange || shootCam.inMinRange;
    }

    public bool TargetInRange()
    {
        return (InRange() && shootCam.TargetHit());
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

    public float GetMaxRange()
    {
        return maxRange;
    }

    public void SetMaxRange(float newVal)
    {
        maxRange = newVal;
    }

    public void OnStop()
    {
        // on stop, unpack any muzzle flashes and set it back to normal layer
        for(int i = shootPoint.childCount-1; i >= 0; i--)
        {
            VFXPooler.instance.ReturnVFX(shootPoint.GetChild(i).gameObject);
        }
    }

    public void OnResume()
    {
        return;
    }
}
