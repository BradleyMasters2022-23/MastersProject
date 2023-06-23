/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - June 20th, 2023
 * Last Edited - June 20th, 2023 by Ben Schuster
 * Description - Character controller with added tutorial functionlaity
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerTargetTutorial : PlayerTarget
{
    [Header("Tutorial")]

    [SerializeField] float missingHealthOnStart;
    [SerializeField] float missingTimeOnStart;

    [Tooltip("Any events executed on player death in the tutorial")]
    [SerializeField] UnityEvent onTutorialDeathEvents;

    /// <summary>
    /// In addition to core systems, apply damage and time damages on start for tutorial sakes
    /// </summary>
    protected override void Start()
    {
        base.Start();
        damagedSoundCooldownTracker.ResetTimer();

        // apply damage to healthbar 0, bypassing shield
        _healthManager.ResourceBarAtIndex(0).Decrease(missingHealthOnStart);
        timestop.DrainGauge(missingTimeOnStart);
    }

    /// <summary>
    /// On death, initiate the tutorial sequence
    /// </summary>
    protected override void KillTarget()
    {
        StartCoroutine(TutorialDeathSequence());
    }

    /// <summary>
    /// Coroutine handling what happens when the player dies in the tutorial
    /// </summary>
    /// <returns></returns>
    private IEnumerator TutorialDeathSequence()
    {
        GameManager.controls.Disable();
        yield return HUDFadeManager.instance.FadeIn();
        //SetForClear();

        // teleport player up to avoid any potential sound weirdness
        transform.position = Vector3.up * 100;
        Time.timeScale = 0;
        onTutorialDeathEvents.Invoke();
    }

    /// <summary>
    /// End the tutorial and go to the hub world. Called via event after cutscene
    /// </summary>
    public void EndTutorial()
    {
        MapLoader.instance.ClearRunData();
        //Destroy(MapLoader.instance.gameObject);
        //SetForClear();
        GameManager.instance.GoToHub();
    }
}
