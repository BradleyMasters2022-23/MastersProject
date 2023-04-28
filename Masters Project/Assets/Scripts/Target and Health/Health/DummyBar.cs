/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - April 26, 2022
 * Last Edited - April 26, 2022 by Ben Schuster
 * Description - Dummy healthbar meant to mimic regeneration. Used by boss phase changes
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DummyBar : MonoBehaviour
{
    [Tooltip("Time it takes to regen")]
    [SerializeField] private float regenTime;

    [SerializeField] private Slider slider;

    [SerializeField] private float startDelay;

    public void RegenHealthbar()
    {
        // reset value, enable
        slider.value = 0;
        gameObject.SetActive(true);

        StartCoroutine(Regen());
    }

    private IEnumerator Regen()
    {
        ScaledTimer t = new ScaledTimer(startDelay, false);

        yield return new WaitUntil(t.TimerDone);

        t.ResetTimer(regenTime);

        while (slider.value < slider.maxValue)
        {
            slider.value = t.TimerProgress();
            yield return new WaitForEndOfFrame();
        }

        // Turn off healthbar
        gameObject.SetActive(false);
        yield return null;
    }
}
