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
    [Header("=====Gameplay=====")]

    [Tooltip("Damage of the bullets this gun fires")]
    [SerializeField] private UpgradableInt damageMultiplier;
    [Tooltip("Speed of the bullets this gun fires")]
    [SerializeField] private UpgradableFloat speedMultiplier;
    [Tooltip("Seconds takes between each shot")]
    [SerializeField] private UpgradableFloat fireDelay;
    [Tooltip("Projectile thats fired from this gun")]
    [SerializeField] private GameObject shotPrefab;
    [Tooltip("Minimum range for aiming to take effect. " +
        "Prevents weird aiming when too close to a wall")]
    [SerializeField] private float minAimRange;

    [Tooltip("Sound that happens when gun go pew pew")]
    [SerializeField] private AudioClip[] gunshotSound;
    private AudioSource source;

    [Header("=====Setup=====")]

    [Tooltip("Where the shot fires from")]
    [SerializeField] private Transform shootPoint;
    [Tooltip("Default target")]
    [SerializeField] private Transform defaultTarget;
    [Tooltip("Layers for raycast to ignore")]
    [SerializeField] LayerMask layersToIgnore;

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
        controller = new GameControls();
        shoot = controller.PlayerGameplay.Shoot;
        shoot.Enable();
        shoot.started += ToggleTrigger;
        shoot.canceled += ToggleTrigger;

        // Initialize upgradable variables
        damageMultiplier.Initialize();
        speedMultiplier.Initialize();
        fireDelay.Initialize();

        // Initialize timers
        fireTimer = new ScaledTimer(fireDelay.Current, false);

        source = gameObject.AddComponent<AudioSource>();
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
        // Shoot projectile, aiming towards passed in target
        GameObject newShot = Instantiate(shotPrefab, shootPoint.position, transform.rotation);
        newShot.transform.LookAt(shootCam.TargetPos);
        newShot.GetComponent<RangeAttack>().Initialize(damageMultiplier.Current, speedMultiplier.Current);

        
        source.PlayOneShot(gunshotSound[Random.Range(0, gunshotSound.Length)],0.3f);
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
    /// Re-enable shooting input if enabled and not null
    /// </summary>
    private void OnEnable()
    {
        if (shoot != null)
            shoot.Enable();
    }
    /// <summary>
    /// Disable input to prevent crashing
    /// </summary>
    private void OnDisable()
    {
        shoot.Disable();
    }
}
