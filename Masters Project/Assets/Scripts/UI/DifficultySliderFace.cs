using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultySliderFace : MonoBehaviour
{
    [SerializeField] Image renderImage;
    [SerializeField] Slider targetSlider;
    [SerializeField] Vector2[] numberRange;
    [SerializeField] Sprite[] icons;

    private void OnEnable()
    {
        if (targetSlider != null)
        {
            targetSlider.onValueChanged.AddListener(UpdateImage);
            UpdateImage(targetSlider.value);
        }
    }
    private void OnDisable()
    {
        if (targetSlider != null)
        {
            targetSlider.onValueChanged.RemoveListener(UpdateImage);
        }
    }

    private void UpdateImage(float newVal)
    {
        int idx;
        for(idx = 0; idx < numberRange.Length; idx++)
        {
            if (newVal >= numberRange[idx].x && newVal < numberRange[idx].y) break;
        }

        if (renderImage.sprite != icons[idx])
        {
            renderImage.sprite = icons[idx];
        }
    }
}
