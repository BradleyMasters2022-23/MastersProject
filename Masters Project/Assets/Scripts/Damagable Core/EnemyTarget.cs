/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - February 2nd, 2022
 * Last Edited - February 2nd, 2022 by Ben Schuster
 * Description - Controls concrete enemy-based targets
 * ================================================================================================
 */
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyTarget : Target
{
    [Header("Enemy Target Info")]
    
    [Tooltip("The VFX for the player dying")]
    [SerializeField] private GameObject deathVFX;

    [Header("Time Orbs")]

    [Tooltip("Prefab of the time orb itself")]
    [SerializeField, AssetsOnly] private GameObject timeOrb;

    [Tooltip("Chance of dropping any time orbs at all")]
    [SerializeField, Range(0, 100)] private float dropChance;

    [Tooltip("If told to drop, what is the range of orbs that can drop")]
    [SerializeField] private Vector2 dropNumerRange;

    /// <summary>
    /// Input stuff for debug kill command
    /// </summary>
    private GameControls controls;
    private InputAction endEncounter;

    /// <summary>
    /// Get reference to the debug kill cheat
    /// </summary>
    private void Start()
    {
        controls = GameManager.controls;
        endEncounter = controls.PlayerGameplay.ClearEncounter;
        endEncounter.performed += DebugKill;
        endEncounter.Enable();
    }

    /// <summary>
    /// Kill the current target, modified to work with enemies
    /// </summary>
    protected override void KillTarget()
    {
        endEncounter.Disable();
        endEncounter.performed -= DebugKill;

        DropOrbs(timeOrb, dropChance, dropNumerRange);

        if (deathSound != null)
            AudioSource.PlayClipAtPoint(deathSound, _center.position);

        // Play death VFX at center of enemy
        if (deathVFX != null)
            Instantiate(deathVFX, _center.position, transform.rotation);

        // Try telling spawn manager to destroy self, if needed
        if (SpawnManager.instance != null)
            SpawnManager.instance.DestroyEnemy();

        // Destroy object, later make pooler
        Destroy(this.gameObject);
    }

    /// <summary>
    /// Cheat kill command
    /// </summary>
    /// <param name="c"></param>
    private void DebugKill(InputAction.CallbackContext c)
    {
        // Dont kill godmode enemies
        if (_healthManager.God)
        {
            return;
        }

        KillTarget();
    }
}
