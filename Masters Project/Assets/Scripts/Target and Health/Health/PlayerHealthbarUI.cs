/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - Februrary 1st, 2022
 * Last Edited - Februrary 1st, 2022 by Ben Schuster
 * Description - Controls slider UI regarding a resource bar, specifically player health
 * ================================================================================================
 */
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealthbarUI : ResourceBarUI
{
    #region Impulse Effects

    [Header("Event HUD Impulse Effects")]
    [SerializeField] protected ImpulseEffect effectSource;
    [HideIf("@this.effectSource == null")]
    [SerializeField] protected Color replenishColor;
    [HideIf("@this.effectSource == null")]
    [SerializeField] protected Color decreaseColor;
    [HideIf("@this.effectSource == null")]
    [SerializeField] protected Color fullColor;
    [HideIf("@this.effectSource == null")]
    [SerializeField] protected Color emptyColor;

    [Tooltip("Sound while bar is recharging"), HideIf("@this.effectSource == null")]
    [SerializeField] protected AudioClipSO barRefill;
    [Tooltip("Sound when bar is full"), HideIf("@this.effectSource == null")]
    [SerializeField] protected AudioClipSO barFull;
    [Tooltip("Sound while bar is reduced"), HideIf("@this.effectSource == null")]
    [SerializeField] protected AudioClipSO barReduce;
    [Tooltip("Sound when bar is emptied"), HideIf("@this.effectSource == null")]
    [SerializeField] protected AudioClipSO barEmpty;
    protected AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    public void OnDecrease()
    {
        // Do any other visual representation when the value decreases
        if (effectSource != null && decreaseColor != null)
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
        if (effectSource != null && emptyColor != null)
        {
            effectSource.ActivateImpulse(emptyColor);
        }

        barEmpty.PlayClip(source);
    }



    #endregion

    #region Critical Health Mode
    
    [Header("Critical Mode")]

    [Tooltip("Whether this healthbar should use a critical state")]
    [SerializeField] bool useCriticalState;
    [Tooltip("Threshold used for critical state. % based"), Range(0, 100)]
    [SerializeField] float criticalThreshold;

    [SerializeField] Color criticalColor;
    [SerializeField] AudioClipSO criticalHealthSFX;
    [SerializeField] UnityEvent onCriticalState;
    [SerializeField] UnityEvent onNormalState;
    bool inCritState = false;


    /// <summary>
    /// Update the color to the critical state
    /// </summary>
    public void UpdateCriticalColor()
    {
        Color targetCol = inCritState ? criticalColor : _displayData._fillColor;
        fillArea.color = targetCol;
    }
    /// <summary>
    /// Play the critical health SFX
    /// </summary>
    public void PlayCriticalHealthSFX()
    {
        criticalHealthSFX.PlayClip(source);
    }

    #endregion

    protected override void UpdateCurrVal()
    {
        bool changed = _targetData.CurrentValue() != _mainSlider.value;

        base.UpdateCurrVal();

        if (changed)
        {
            if (_mainSlider.value <= 0)
                OnDeplete();
            else if (_mainSlider.value >= _mainSlider.maxValue)
                OnFull();
            else if (_mainSlider.value <= lastVal)
                OnDecrease();
            else if (_mainSlider.value > lastVal)
                OnIncrease();
        }

        if(useCriticalState)
        {
            float currPercent = (_targetData.CurrentValue() / _mainSlider.maxValue) * 100;

            // check if exiting crit state
            if (inCritState && (currPercent > criticalThreshold))
            {
                inCritState = false;
                onNormalState.Invoke();
            }
            // check if entering crit state
            else if (!inCritState && (currPercent <= criticalThreshold))
            {
                inCritState = true;
                onCriticalState.Invoke();
            }
        }
    }
    

}
