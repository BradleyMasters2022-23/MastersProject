/* ================================================================================================
 * Author - Ben Schuster   
 * Date Created - June 12th, 2023
 * Last Edited - June 12th, 2023 by Ben Schuster
 * Description - Flashes an image item through various colors
 * ================================================================================================
 */using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class FlashProtocol : MonoBehaviour
{
    /// <summary>
    /// Image to change colors of
    /// </summary>
    private Image img;
    [Tooltip("Time between each color change")]
    [SerializeField] private float flashRate;

    [Tooltip("Set of colors to flash through")]
    [SerializeField] private Color[] flashColors;
    /// <summary>
    /// The original color of the object
    /// </summary>
    [SerializeField] private Color originalColor;
    /// <summary>
    /// Reference to the routine
    /// </summary>
    private Coroutine flashRoutine;
    /// <summary>
    /// Activation status of the object before the flash routine began
    /// </summary>
    [SerializeField] private bool preActivationStatus;



    /// <summary>
    /// Begin flashing through all colors 
    /// </summary>
    public void BeginFlash()
    {
        // Don't start routine if one is already going
        if (flashRoutine != null)
            return;

        if(img == null)
            img = GetComponent<Image>();

        originalColor = img.color;

        preActivationStatus = gameObject.activeInHierarchy;
        gameObject.SetActive(true);
        flashRoutine = StartCoroutine(Flash());
    }
    /// <summary>
    /// Repeatedly flash through all colors
    /// </summary>
    /// <returns></returns>
    private IEnumerator Flash()
    {
        int colIndex = 0;
        while(true)
        {
            img.color = flashColors[colIndex];
            colIndex = (colIndex+1) % flashColors.Length;

            yield return new WaitForSecondsRealtime(flashRate);
        }
    }
    /// <summary>
    /// Stop flashing, turn off object
    /// </summary>
    public void StopFlash()
    {
        // only revert if the routine was active
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
            flashRoutine = null;

            img.color = originalColor;
            gameObject.SetActive(preActivationStatus);
        }
    }
}
