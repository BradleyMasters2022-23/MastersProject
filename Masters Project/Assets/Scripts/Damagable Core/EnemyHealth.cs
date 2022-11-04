/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 26th, 2022
 * Last Edited - October 26th, 2022 by Ben Schuster
 * Description - Controller for enemy health
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

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

    [Tooltip("Range of velocity items can drop with")]
    [SerializeField] private Vector2 dropVelocityRange;

    [Tooltip("Range of velocity items can drop with")]
    [SerializeField] private Vector2 dropXAngleRange;


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
        DropOrbs();

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
        // TODO - CONVERT TO CHANNEL
        SpawnManager manager = FindObjectOfType<SpawnManager>();
        if (manager != null)
        {
            manager.DestroyEnemy();
        }

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
            // Randomly generate velocity and rotation angles
            float vel = Random.Range(dropVelocityRange.x, dropVelocityRange.y);
            float angY = Random.Range(0, 360);
            float angX = Random.Range(dropXAngleRange.x, dropXAngleRange.y);

            Quaternion rot = Quaternion.Euler(angX, angY, 0);

            // Spawn objects, apply rotation and velocity
            GameObject orb = Instantiate(timeOrb, transform.position, rot);
            orb.GetComponent<Rigidbody>().velocity = orb.transform.forward * vel;
        }
    }
}
