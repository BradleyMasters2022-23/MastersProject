using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlowFade : MonoBehaviour
{
    /// <summary>
    ///  Current opacity
    /// </summary>
    private float opacity;

    /// <summary>
    /// default color
    /// </summary>
    Color normColor;


    private void Start()
    {
        normColor = GetComponent<Image>().color;
    }

    /// <summary>
    /// Fade color as it transitions
    /// </summary>
    private void Update()
    {
        float time = TimeManager.WorldTimeScale;

        if (opacity != (1 - time))
        {
            opacity = (1 - time);
            Color temp = new Color(normColor.r, normColor.g, normColor.b, opacity);
            GetComponent<Image>().color = temp;
        }
    }
}
