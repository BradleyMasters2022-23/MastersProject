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

    [SerializeField] private Image fillArea;
    [SerializeField] private Image emptyArea;
    [SerializeField] private bool scaleWithMax;
    
    private float pixelsPerHealth;

    
    [Header("Event HUD Impulse Effects")]
    [SerializeField] private ImpulseEffect effectSource;
    [HideIf("@this.effectSource == null")]
    [SerializeField] private Color replenishColor;
    [HideIf("@this.effectSource == null")]
    [SerializeField] private Color decreaseColor;
    [HideIf("@this.effectSource == null")]
    [SerializeField] private Color fullColor;
    [HideIf("@this.effectSource == null")]
    [SerializeField] private Color emptyColor;

    [Tooltip("Sound while bar is recharging"), HideIf("@this.effectSource == null")]
    [SerializeField] private AudioClipSO barRefill;
    [Tooltip("Sound when bar is full"), HideIf("@this.effectSource == null")]
    [SerializeField] private AudioClipSO barFull;
    [Tooltip("Sound while bar is reduced"), HideIf("@this.effectSource == null")]
    [SerializeField] private AudioClipSO barReduce;
    [Tooltip("Sound when bar is emptied"), HideIf("@this.effectSource == null")]
    [SerializeField] private AudioClipSO barEmpty;
    private AudioSource source;

    protected virtual void Awake()
    {
        source = gameObject.AddComponent<AudioSource>();
    }

    protected virtual void OnEnable()
    {
        if(!initialized)
            StartCoroutine(TryInitialize());
    }

    private IEnumerator TryInitialize()
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

    private void UpdateCurrVal()
    {
        if (_targetData.CurrentValue() != _mainSlider.value)
        {
            lastVal = _mainSlider.value;
            _mainSlider.value = _targetData.CurrentValue();

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
        if(_mainSlider.maxValue != _targetData.MaxValue())
        {
            _mainSlider.maxValue = _targetData.MaxValue();
            coreReference.sizeDelta = new Vector2(pixelsPerHealth * _mainSlider.maxValue, coreReference.sizeDelta.y);
        }
    }

    public void OnDecrease()
    {
        // Do any other visual representation when the value decreases
        if (effectSource!=null && decreaseColor!=null)
        {
            effectSource.ActivateImpulse(decreaseColor);
        }

        barReduce.PlayClip(source);
    }

    public void OnIncrease()
    {
        // Do any other visual representation when the value increases
        if (effectSource != null && replenishColor != null)
        {
            effectSource.ActivateImpulse(replenishColor);
        }

        barRefill.PlayClip(source);
    }

    public void OnFull()
    {
        // Do any other visual representation on fill
        if (effectSource != null && fullColor != null)
        {
            effectSource.ActivateImpulse(fullColor);
        }

        barFull.PlayClip(source);
    }

    public void OnDeplete()
    {
        // Do any other visual representation on deplete
        if(effectSource!=null && emptyColor!=null)
        {
            effectSource.ActivateImpulse(emptyColor);
        }

        barEmpty.PlayClip(source);
    }

    
}
