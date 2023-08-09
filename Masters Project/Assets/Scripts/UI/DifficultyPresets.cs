using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyPresets : MonoBehaviour
{
    [SerializeField] string difficultyName;

    [SerializeField] Slider[] difficultySliders;
    [SerializeField] float[] difficultyValues;

    public void SetDifficultyPreset()
    {
        for(int i = 0; i < difficultyValues.Length; i++)
        {
            if (i >= difficultySliders.Length) return;

            difficultySliders[i].value = difficultyValues[i];
        }
    }
}
