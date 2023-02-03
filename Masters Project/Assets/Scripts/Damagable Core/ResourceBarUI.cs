/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - Februrary 1st, 2022
 * Last Edited - Februrary 1st, 2022 by Ben Schuster
 * Description - Controls slider UI regarding a resource bar
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceBarUI : MonoBehaviour
{
    [Tooltip("The target resourcebar to visually display")]
    [SerializeField] private ResourceBar _targetResource;
    [Tooltip("The slider to display the data to")]
    [SerializeField] private Slider _mainSlider;

    private float lastVal;

    private void Update()
    {
        UpdateCoreSlider();
        UpdateCurrVal();
    }

    private void UpdateCurrVal()
    {
        if (_targetResource.CurrentValue() != lastVal)
        {
            lastVal = _mainSlider.value;
            _mainSlider.value = _targetResource.CurrentValue();

            if (_mainSlider.value <= 0)
                OnDeplete();
            else if (_mainSlider.value >= _mainSlider.maxValue)
                OnFull();
            else if (_mainSlider.value <= lastVal)
                OnDecrease();
            else if (_mainSlider.value > lastVal)
                OnIncrease();
        }
    }

    private void UpdateCoreSlider()
    {
        if(_mainSlider.maxValue != _targetResource.MaxValue())
        {
            _mainSlider.maxValue = _targetResource.MaxValue();
        }
    }

    public void OnDecrease()
    {
        // Do any other visual representation when the value decreases
    }

    public void OnIncrease()
    {
        // Do any other visual representation when the value increases
    }

    public void OnFull()
    {
        // Do any other visual representation on fill
    }

    public void OnDeplete()
    {
        // Do any other visual representation on deplete 
    }
}
