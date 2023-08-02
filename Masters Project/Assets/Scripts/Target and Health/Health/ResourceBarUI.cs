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
using Sirenix.OdinInspector;

public class ResourceBarUI : MonoBehaviour
{
    [Header("Data and target info")]

    protected ResourceBar _targetData;
    protected ResourceBarSO _displayData;
    [Tooltip("The target resourcebar to visually display")]
    [SerializeField] protected HealthManager _targetResource;
    [Tooltip("The target index of health manager's healthbars to display")]
    [SerializeField] protected int _targetIndex = 0;
    [Tooltip("The slider to display the data to")]
    [SerializeField] protected Slider _mainSlider;

    protected float lastVal;

    protected bool initialized = false;

    [Header("Display info")]

    [SerializeField] protected RectTransform coreReference;

    [SerializeField] protected Image fillArea;
    [SerializeField] protected Image emptyArea;
    [SerializeField] protected bool scaleWithMax;
    
    protected float pixelsPerHealth;

    protected virtual void OnEnable()
    {
        if(!initialized)
            StartCoroutine(TryInitialize());
    }

    protected IEnumerator TryInitialize()
    {
        int i = 0;
        while(_targetData == null)
        {
            if(_targetResource!= null && _targetResource.Initialized)
                _targetData = _targetResource.ResourceBarAtIndex(_targetIndex);

            yield return new WaitForSecondsRealtime(0.1f);
            yield return null;

            i++;
            if (i >= 1000)
            {
                this.enabled = false;
                yield break;
            }
        }

        _displayData = _targetData.BarData;

        fillArea.color = _displayData._fillColor;
        emptyArea.color = _displayData._emptyColor;

        _mainSlider.maxValue= _targetData.MaxValue();
        _mainSlider.value = _targetData.CurrentValue();

        GetScalingData();

        initialized = true;
        yield return null;
    }

    protected void GetScalingData()
    {
        // calculate the original scale of it currently
        if (!scaleWithMax)
        {
            return;
        }

        pixelsPerHealth = coreReference.sizeDelta.x / _mainSlider.maxValue;
    }
    protected virtual void LateUpdate()
    {
        if (!initialized)
            return;

        UpdateCoreSlider();
        UpdateCurrVal();
    }

    protected virtual void UpdateCoreSlider()
    {
        if (_mainSlider.maxValue != _targetData.MaxValue())
        {
            _mainSlider.maxValue = _targetData.MaxValue();
            coreReference.sizeDelta = new Vector2(pixelsPerHealth * _mainSlider.maxValue, coreReference.sizeDelta.y);
        }
    }

    protected virtual void UpdateCurrVal()
    {
        if (_targetData.CurrentValue() != _mainSlider.value)
        {
            lastVal = _mainSlider.value;
            _mainSlider.value = _targetData.CurrentValue();
        }
    }
}
