/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - July 28th, 2023
 * Last Edited - July 28th, 2023 by Ben Schuster
 * Description - Script that observes a slider and prints out its text, with additional functions
 * ================================================================================================
 */
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class SliderValueObserver : MonoBehaviour
{
    [Tooltip("Slider value to observe")]
    [SerializeField] Slider targetSlider;
    [SerializeField] float defaultValue = 100;

    /// <summary>
    /// This object's text
    /// </summary>
    private TextMeshProUGUI text;

    [SerializeField] string postText;

    [SerializeField] Color increaseColor;
    [SerializeField] Color decreaseColor;
    private Color defaultColor;

    /// <summary>
    /// make sure reference is got
    /// </summary>
    private void OnEnable()
    {
        if(text == null)
            text = GetComponent<TextMeshProUGUI>();

        defaultColor = text.color;
        targetSlider.onValueChanged.AddListener(OnSliderUpdate);
        OnSliderUpdate(targetSlider.value);
    }

    private void OnDisable()
    {
        text.color = defaultColor;
        targetSlider.onValueChanged.RemoveListener(OnSliderUpdate);
    }

    /// <summary>
    /// When value is changed, update text value
    /// </summary>
    public void OnSliderUpdate(float newVal)
    {
        if (text == null) return;

        // build text
        text.text = newVal.ToString() + postText;

        Color newCol;
        // build color
        if (newVal >= defaultValue)
        {
            newCol = Color.Lerp(defaultColor, increaseColor, (newVal - defaultValue) / (targetSlider.maxValue - defaultValue));
        }
        else
        {
            newCol = Color.Lerp(decreaseColor, defaultColor, (newVal - targetSlider.minValue) / (defaultValue - targetSlider.minValue));
        }

        text.color = newCol;
    }
}
