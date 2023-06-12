using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class FlashProtocol : MonoBehaviour
{
    private Image img;

    [SerializeField] private float flashRate;
    [SerializeField] private Color[] flashColors;
    private Color originalColor;
    private int colIndex;

    private Coroutine flashRoutine;

    /// <summary>
    /// Begin flashing through all colors 
    /// </summary>
    public void BeginFlash()
    {
        if(img == null)
            img = GetComponent<Image>();

        originalColor= img.color;

        gameObject.SetActive(true);
        StartCoroutine(Flash());
    }
    /// <summary>
    /// Repeatedly flash through all colors
    /// </summary>
    /// <returns></returns>
    private IEnumerator Flash()
    {
        while(true)
        {
            img.color = flashColors[colIndex];
            colIndex = (colIndex++) % flashColors.Length;

            yield return new WaitForSecondsRealtime(flashRate);
        }
    }
    /// <summary>
    /// Stop flashing, turn off object
    /// </summary>
    public void StopFlash()
    {
        gameObject.SetActive(false);

        if(flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }

        img.color = originalColor;
        colIndex = 0;
    }
}
