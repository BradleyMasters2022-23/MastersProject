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
using static Cinemachine.DocumentationSortingAttribute;
using UnityEngine.Rendering;

public class ResourceBarUI : MonoBehaviour
{
    [Header("Data and target info")]

    private ResourceBar _targetData;
    private ResourceBarSO _displayData;
    [Tooltip("The target resourcebar to visually display")]
    [SerializeField] private HealthManager _targetResource;
    [Tooltip("The target index of health manager's healthbars to display")]
    [SerializeField] private int _targetIndex = 0;
    [Tooltip("The slider to display the data to")]
    [SerializeField] private Slider _mainSlider;

    private float lastVal;

    private bool initialized = false;

    [Header("Display info")]

    [SerializeField] private Image fillArea;
    [SerializeField] private Image emptyArea;

    [SerializeField] private bool scaleWithMax;
    [HideIf("@this.scaleWithMax == false")]
    [SerializeField] private RectTransform coreReference;
    private float pixelsPerHealth;

    [Header("Event HUD Impulse Effects")]
    [SerializeField] private ImpulseEffect effectSource;
    [SerializeField] private Color replenishColor;
    [SerializeField] private Color decreaseColor;
    [SerializeField] private Color fullColor;
    [SerializeField] private Color emptyColor;

    [Tooltip("Sound while bar is recharging")]
    [SerializeField] private AudioClip BarRefill;
    [Tooltip("Sound when bar is full")]
    [SerializeField] private AudioClip BarFull;
    private AudioSource source;

    private void Start()
    {
        StartCoroutine(TryInitialize());
    }

    private IEnumerator TryInitialize()
    {
        int i = 0;
        while(_targetData == null)
        {
            _targetData = _targetResource.ResourceBarAtIndex(_targetIndex);

            yield return new WaitForSecondsRealtime(0.5f);
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

    private void GetScalingData()
    {
        // calculate the original scale of it currently
        if (!scaleWithMax)
        {
            return;
        }

        pixelsPerHealth = coreReference.sizeDelta.x / _mainSlider.maxValue;
    }
    private void LateUpdate()
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
        //Debug.Log($"Decrease called from {gameObject.name}");
        // Do any other visual representation when the value decreases
        if (effectSource!=null && decreaseColor!=null)
        {
            effectSource.ActivateImpulse(decreaseColor);
        }
    }

    public void OnIncrease()
    {
        //Debug.Log($"Replenish called from {gameObject.name}");
        // Do any other visual representation when the value increases
        if (effectSource != null && replenishColor != null)
        {
            effectSource.ActivateImpulse(replenishColor);
        }

        if (BarRefill != null)
        {
            source.PlayOneShot(BarRefill, 1f);
        }
    }

    public void OnFull()
    {
        // Do any other visual representation on fill
        if (effectSource != null && fullColor != null)
        {
            effectSource.ActivateImpulse(fullColor);
        }
        if (BarFull != null)
        {
            source.PlayOneShot(BarFull, 1f);
        }
    }

    public void OnDeplete()
    {
        // Do any other visual representation on deplete
        if(effectSource!=null && emptyColor!=null)
        {
            effectSource.ActivateImpulse(emptyColor);
        }
    }

    public void Awake()
    {
        source = gameObject.AddComponent<AudioSource>();
    }
}
