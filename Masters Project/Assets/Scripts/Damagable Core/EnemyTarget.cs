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
        // remove cheat to prevent bugs
        endEncounter.performed -= DebugKill;

        // Try telling spawn manager to destroy self, if needed
        if (SpawnManager.instance != null)
            SpawnManager.instance.DestroyEnemy();

        base.KillTarget();
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
