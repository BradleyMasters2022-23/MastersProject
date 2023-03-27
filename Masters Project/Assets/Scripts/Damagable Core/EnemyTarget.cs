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
    #region Pooling
    /// <summary>
    /// Core data used by enemy
    /// </summary>
    private EnemySO enemyData;

    /// <summary>
    /// Return this object to its pool
    /// </summary>
    public void ReturnToPool()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Retrieve from pool
    /// </summary>
    public void PullFromPool(EnemySO data)
    {
        if (enemyData == null)
            enemyData = data;

        _killed = false;
        _healthManager.ResetHealth();
        gameObject.SetActive(true);
        // TODO - get data from difficulty scaler
    }
    #endregion

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
    /// Return enemy to pool instead of destroying it
    /// </summary>
    protected override void DestroyObject()
    {
        EnemyPooler.instance.Return(enemyData, gameObject);
    }

    #region Cheats
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
    #endregion
}
