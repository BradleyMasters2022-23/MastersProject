/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - March 4th, 2022
 * Last Edited - March 4th, 2022 by Ben Schuster
 * Description - Manages red tint on the regenerating shields
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldVisualIndicator : MonoBehaviour
{
    [SerializeField] private HealthManager _targetShield;
    [SerializeField] private MeshRenderer _redTintRenderer;
    private const int ShieldIndex = 0;
    private float maxHealth;
    private float currHealth;

    private Color baseColor;

    private float minimumRedTime = 0.5f;
    private float minimumRedTint = 0.3f;

    private ScaledTimer minRedTimer;

    private void Start()
    {
        if(_redTintRenderer != null)
        {
            _redTintRenderer.gameObject.SetActive(true);
            baseColor = _redTintRenderer.material.color;
        }

        if(_targetShield != null)
        {
            maxHealth = _targetShield.MaxHealth(ShieldIndex);
            currHealth = _targetShield.CurrentHealth(ShieldIndex);
        }

        minRedTimer = new ScaledTimer(minimumRedTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (_targetShield == null  || !_targetShield.isActiveAndEnabled || _redTintRenderer == null)
            return;

        currHealth = _targetShield.CurrentHealth(ShieldIndex);

        float ratio = 1 - (currHealth / (float)maxHealth);

        if(ratio <= minimumRedTint && ratio != 0)
            baseColor.a = minimumRedTint;
        else
            baseColor.a = ratio;

        _redTintRenderer.material.color = baseColor;
    }
}
