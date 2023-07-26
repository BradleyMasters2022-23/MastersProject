using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossColorSwapper : MonoBehaviour
{
    [SerializeField] float transitionTime;

    [SerializeField] Color[] phaseColors;

    [SerializeField] MeshRenderer[] affectedRenderers;

    /// <summary>
    /// Change the color to the next target color
    /// </summary>
    /// <param name="phase">Phase color to use</param>
    public void SetPhaseColor(int phase)
    {
        StartCoroutine(LerpColor(Mathf.Clamp(phase, 0, affectedRenderers.Length)));
    }

    /// <summary>
    /// Lerp the color over a time
    /// </summary>
    /// <param name="phase">Phase color to uise</param>
    /// <returns></returns>
    private IEnumerator LerpColor(int phase)
    {
        int cnt = affectedRenderers.Length;
        float t = 0;
        Color originalColor = affectedRenderers[0].material.color;
        Color targetColor = phaseColors[phase];
        while (t < transitionTime)
        {
            for (int i = 0; i < cnt; i++)
            {
                affectedRenderers[i].material.color = Color.Lerp(originalColor, targetColor, t / transitionTime);
                t += Time.deltaTime;
                yield return null;
            }
        }
    }
}
