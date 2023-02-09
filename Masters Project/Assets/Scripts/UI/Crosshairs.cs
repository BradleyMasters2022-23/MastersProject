using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshairs : MonoBehaviour
{
    [SerializeField] private PlayerGunController gunRef;
    [SerializeField] private AnimationCurve scaleOverBloom;
    private float maxBloom;
    private float minBloom;
    private float lastBloom;
    private float enhancedThreshold;
    private Vector3 scaleRef;
    [SerializeField] private RectTransform crosshair;

    [Header("Crosshair distance indicators")]
    [SerializeField] private Color inRangeColor;
    [SerializeField] private Color outRangeColor;
    [SerializeField] private Image[] crosshairImages;

    private void Start()
    {
        if(gunRef == null)
        {
            Debug.LogError("Crosshair does not have gun reference set! Destroying self to prevent errors.");
            Destroy(this); 
            return;
        }

        // get necessary data to determine the scale and base points on the curve
        maxBloom = gunRef.MaxBloom;
        minBloom = gunRef.BaseBloom;
        enhancedThreshold = gunRef.EnhancedShotThreshold;
    }

    private void LateUpdate()
    {
        if(TimeManager.WorldTimeScale > enhancedThreshold)
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
        else if(scaleRef != Vector3.zero)
        {
            scaleRef = Vector3.zero;
            crosshair.localScale = scaleRef;
        }

        if (crosshairImages.Length > 0)
        {
            if(gunRef.InRange() && crosshairImages[0].color != inRangeColor)
            {
                foreach(Image img in crosshairImages)
                {
                    img.color = inRangeColor;
                }
            }
            else if(!gunRef.InRange() && crosshairImages[0].color != outRangeColor)
            {
                foreach (Image img in crosshairImages)
                {
                    img.color = outRangeColor;
                }
            }
        }

        
    }
}
