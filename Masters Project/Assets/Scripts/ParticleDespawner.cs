/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 26th, 2022
 * Last Edited - October 26th, 2022 by Ben Schuster
 * Description - Manages slowing particles down in real time
 * ================================================================================================
 */
using UnityEngine;

public class ParticleDespawner : MonoBehaviour
{
    /// <summary>
    /// Reference to the main module of the particle system
    /// </summary>
    private ParticleSystem.MainModule main;

    /// <summary>
    /// Whether this particle system loops
    /// </summary>
    private bool loops;

    /// <summary>
    /// tracker for lifetime
    /// </summary>
    private ScaledTimer lifeTimer;

    /// <summary>
    /// Get necessary references 
    /// </summary>
    void Awake()
    {
        main = GetComponent<ParticleSystem>().main;

        loops = main.loop;

        lifeTimer = new ScaledTimer(main.duration);
    }

    void Update()
    {
        // Update timeline, check if timer is done
        main.simulationSpeed = 1 * TimeManager.WorldTimeScale;

        // If not looping and reached its duration, destroy self
        if (!loops && lifeTimer.TimerDone())
            Destroy(gameObject);
    }
}
