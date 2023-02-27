using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent((typeof(VisualEffect)))]
public class VFXTimeScaler : MonoBehaviour
{
    private VisualEffect effectRef;
    private float lastTimeScale = 1;
    public float fastForwardTime = 0.5f;

    private void Awake()
    {
        effectRef= GetComponent<VisualEffect>();

        if (TimeManager.WorldTimeScale <= 0.15f)
            FastForward();

    }

    // Update is called once per frame
    void Update()
    {
        if (lastTimeScale != TimeManager.WorldTimeScale)
        {
            effectRef.playRate = 1 * TimeManager.WorldTimeScale;
        }

        lastTimeScale = TimeManager.WorldTimeScale;
    }

    private void FastForward()
    {
        effectRef.Play();
        effectRef.Simulate(fastForwardTime);
    }
}
