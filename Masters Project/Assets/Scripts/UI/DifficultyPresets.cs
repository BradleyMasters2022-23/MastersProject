using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyPresets : MonoBehaviour
{
    [SerializeField] string difficultyName;
    [SerializeField] SliderSetting[] sliders;
    public void SetDifficultyPreset()
    {
        // set new difficulty
        GlobalDifficultyManager.instance.SetDifficultyMode(difficultyName);

        // tell sliders to update
        foreach(var s in sliders)
        {
            s.LoadSetting();
        }
    }
}
