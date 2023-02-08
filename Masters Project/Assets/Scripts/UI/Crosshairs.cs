using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        
    }
}
