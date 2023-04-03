/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - January 30th, 2022
 * Last Edited - January 30th, 2022 by Ben Schuster
 * Description - Manages the player's crosshair functionality
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshairs : MonoBehaviour
{
    [SerializeField] private PlayerGunController gunRef;
    [SerializeField] private AnimationCurve scaleOverBloom;
    private float maxBloom;
    private Vector3 scaleRef;
    [SerializeField] private RectTransform crosshair;

    [Header("Crosshair distance indicators")]
    [SerializeField] private Color targetColor;
    [SerializeField] private Color noTargetColor;

    [SerializeField, Range(0, 1)] private float outRangeAlpha;
    [SerializeField, Range(0, 1)] private float inRangeAlpha;

    [SerializeField] private Image[] crosshairImages;

    [SerializeField] private Image[] lockOnImages;

    private Color currColor;

    private void Start()
    {
        if(gunRef == null)
        {
            Debug.LogError("Crosshair does not have gun reference set! Destroying self to prevent errors.");
            Destroy(this); 
            return;
        }
        maxBloom = gunRef.MaxBloom;
        currColor = new Color(noTargetColor.r, noTargetColor.g, noTargetColor.b, outRangeAlpha);
    }

    private void LateUpdate()
    {
        CrosshairBloomAdjustments();

        CrosshairDistanceAdjustments();
    }

    private void CrosshairBloomAdjustments()
    {
        if (!TimeManager.TimeStopped)
        {
            // Get current bloom, determine if this would be redundent to change
            float currBloom = gunRef.CurrBloom;

            // convert to use the animation curve
            currBloom = scaleOverBloom.Evaluate(currBloom / (maxBloom));
            scaleRef.x = currBloom;
            scaleRef.y = currBloom;
            scaleRef.z = currBloom;
            crosshair.localScale = scaleRef;
        }
        else if (scaleRef != Vector3.zero)
        {
            scaleRef = Vector3.zero;
            crosshair.localScale = scaleRef;
        }
    }

    private void CrosshairDistanceAdjustments()
    {
        if (crosshairImages.Length <= 0)
            return;

        // adjust color based on target being in range 
        if (gunRef.InRange() && gunRef.TargetInRange())
        {
            currColor = targetColor;

            if (lockOnImages.Length > 0 && !lockOnImages[0].enabled)
            {
                foreach(Image img in lockOnImages)
                {
                    img.enabled = true;
                }
            }
        }
        else
        {
            currColor = noTargetColor;

            if (lockOnImages.Length > 0 && lockOnImages[0].enabled)
            {
                foreach (Image img in lockOnImages)
                {
                    img.enabled = false;
                }
            }
        }

        // adjust alpha based on distance
        if (gunRef.InRange())
        {
            currColor.a = inRangeAlpha;
        }
        else
        {
            currColor.a = outRangeAlpha;
        }

        // apply new color
        foreach (Image img in crosshairImages)
        {
            img.color = currColor;
        }
    }
}
