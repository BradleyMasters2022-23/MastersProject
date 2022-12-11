/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 26th, 2022
 * Last Edited - November 4th, 2022 by Ben Schuster
 * Description - Controller for enemy health
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;

public class EnemyHealth : Damagable
{
    [Header("=====Enemy Health=====")]

    [Tooltip("Amount of health this enemy has")]
    [SerializeField] private int maxHealth;

    [Tooltip("Whether this enemy can die")]
    [SerializeField] private bool immortal;

    [Tooltip("The VFX for the player dying")]
    [SerializeField] private GameObject deathVFX;

    [Tooltip("Time it takes after an enemy takes damage before hiding its UI. Set to 0 to leave the UI always on.")]
    [SerializeField] private float hideUIDelay;

    [Header("=====Time Orbs=====")]

    [Tooltip("Prefab of the time orb itself")]
    [SerializeField, AssetsOnly] private GameObject timeOrb;

    [Tooltip("Chance of dropping any time orbs at all")]
    [SerializeField, Range(0, 100)] private float dropChance;

    [Tooltip("If told to drop, what is the range of orbs that can drop")]
    [SerializeField] private Vector2 dropNumerRange;

    [SerializeField] private AudioClip deathSound;
    private AudioSource source;

    /// <summary>
    /// List of active coroutines that need to be stopped on death
    /// </summary>
    protected List<Coroutine> activeRoutines;

    /// <summary>
    /// Current health of this enemy
    /// </summary>
    private float currHealth;

    /// <summary>
    /// UI healthbar for this enemy
    /// </summary>
    private Slider healthbar;

    /// <summary>
    /// Tracker for the hide UI delay
    /// </summary>
    private ScaledTimer hideUITimer;

    /// <summary>
    /// Input stuff for debug kill command
    /// </summary>
    private GameControls controls;
    private InputAction kill;
    private InputAction endEncounter;

    /// <summary>
    /// Get the connected slider object
    /// </summary>
    private void Awake()
    {
        // Initialize values
        healthbar = GetComponentInChildren<Slider>(true);
        hideUITimer = new ScaledTimer(hideUIDelay, false);
        activeRoutines = new List<Coroutine>();
        currHealth = maxHealth;

        // Update slider values to assigned health values
        healthbar.maxValue = maxHealth;
        healthbar.value = maxHealth;

        source = GetComponent<AudioSource>();
    }

    private void Start()
    {
        controls = GameManager.controls;
        kill = controls.PlayerGameplay.Kill;
        kill.performed += DebugKill;
        kill.Enable();
        endEncounter = controls.PlayerGameplay.ClearEncounter;
        endEncounter.performed += DebugKill;
        endEncounter.Enable();
    }

    private void Update()
    {
        // If slider is still visible and timer is done, hide it
        if(healthbar.gameObject.activeInHierarchy && 
            hideUIDelay > 0 && hideUITimer.TimerDone())
        {
            healthbar.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Damage the enemy
    /// </summary>
    /// <param name="_dmg">Damage being delt to enemy</param>
    public override void Damage(int _dmg)
    {
        hideUITimer.ResetTimer();
        healthbar.gameObject.SetActive(true);

        // If the damage exceeds its raining health
        if (currHealth - _dmg <= 0)
        {
            currHealth = 0;
            healthbar.value = currHealth;

            // If set to immortal, do not die
            if (!immortal && !killed)
            {
                killed = true;
                Die();
            }
        }
        // otherwise, just deal damage
        else
        {
            currHealth -= _dmg;
            healthbar.value = currHealth;
        }
    }


    /// <summary>
    /// Kill the enemy
    /// </summary>
    protected override void Die()
    {
        kill.Disable();
        endEncounter.Disable();

        DropOrbs();

        if (deathSound != null)
            AudioSource.PlayClipAtPoint(deathSound, transform.position);

        // Disable all coroutines currently active
        for (int i  = activeRoutines.Count - 1; i >= 0; i--)
        {
            StopCoroutine(activeRoutines[i]);
            activeRoutines.RemoveAt(i);
        }

        // Play death VFX at center of enemy
        if (deathVFX != null)
            Instantiate(deathVFX, transform.position + Vector3.up * (transform.localScale.y / 2), transform.rotation);

        // Try telling spawn manager to destroy self, if needed
        if(SpawnManager.instance != null)
            SpawnManager.instance.DestroyEnemy();

        // Destroy object
        Destroy(this.gameObject);
    }

    /// <summary>
    /// Determine if drops should drop, and spawn them
    /// </summary>
    protected void DropOrbs()
    {
        // Roll the dice. If the value is higher than the value, exit
        if(Random.Range(1, 101) > dropChance || timeOrb is null)
        {
            return;
        }

        int spawnAmount = Random.Range((int) dropNumerRange.x, (int) dropNumerRange.y);

        // Spawn the randomized amount at random ranges, as long as they aren't intersecting
        for(int i = 0; i < spawnAmount; i++)
        {
            // Spawn objects, apply rotation and velocity
            Instantiate(timeOrb, transform.position, Quaternion.identity);
        }
    }


    private void DebugKill(InputAction.CallbackContext c)
    {
        // Dont do anything if its immortal as its a dummy instead
        if (immortal)
            return;

        Die();
    }
}
