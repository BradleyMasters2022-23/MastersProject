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
    [SerializeField] private ParticleSystem[] particles;

    /// <summary>
    /// Whether this particle system loops
    /// </summary>
    private bool loops;

    /// <summary>
    /// tracker for lifetime
    /// </summary>
    private ScaledTimer lifeTimer;

    /// <summary>
    /// Get the prev time scale to check for change
    /// </summary>
    private float lastTimeScale;

    /// <summary>
    /// Get necessary references 
    /// </summary>
    void Awake()
    {
        particles = GetComponentsInChildren<ParticleSystem>();

        loops = particles[0].main.loop;

        float longestDur = float.MinValue;
        
        for(int i = 0; i < particles.Length; i++)
        {
            if (particles[i].main.duration > longestDur)
                longestDur = particles[i].main.duration;
        }

        lifeTimer = new ScaledTimer(longestDur);

        lastTimeScale = 1;
    }

    void Update()
    {
        if(lastTimeScale != TimeManager.WorldTimeScale)
        {
            // Update timeline, check if timer is done
            for (int i = 0; i < particles.Length; i++)
            {
                ParticleSystem.MainModule main = particles[i].main;
                main.simulationSpeed = 1 * TimeManager.WorldTimeScale;
            }
        }

        // If not looping and reached its duration, destroy self
        if (!loops && lifeTimer.TimerDone())
            Destroy(gameObject);

        lastTimeScale = TimeManager.WorldTimeScale;
    }
}
