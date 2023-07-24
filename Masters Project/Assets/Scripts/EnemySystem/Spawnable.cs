/* 
 * ================================================================================================
 * Author - Ben Schuster   
 * Date Created - July 24th, 2023
 * Last Edited - July 24th, 2023 by Ben Schuster
 * Description - Controller for visuals for spawning enemies
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Spawnable : MonoBehaviour
{
    [Tooltip("Animator controller for the spawning effects")]
    [SerializeField] Animator spawnAnimator;
    [Tooltip("Events always called when the animation completes")]
    [SerializeField] UnityEvent standardOnComplete;

    /// <summary>
    /// Any effects to call on complete
    /// </summary>
    UnityAction onAnimationFinish;

    /// <summary>
    /// Start this object's spawning VFX.
    /// </summary>
    /// <param name="onComplete">Action to call once complete</param>
    public void StartSpawning(float speedMod = 1, UnityAction onComplete = null)
    {
        onAnimationFinish = onComplete;
        spawnAnimator.speed *= speedMod;
        spawnAnimator.enabled = true;
    }

    /// <summary>
    /// Call complete actions, if any. Called via animator.
    /// </summary>
    public void AnimationFinish()
    {
        onAnimationFinish?.Invoke();
        standardOnComplete?.Invoke();
    }

    /// <summary>
    /// Finish and remove this spawn effect
    /// </summary>
    public void DespawnEffects()
    {
        spawnAnimator.playbackTime = 1;
        spawnAnimator.enabled = false;
    }
}
