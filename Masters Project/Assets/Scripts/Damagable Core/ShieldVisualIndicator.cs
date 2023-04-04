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

    private void Start()
    {
        if(_redTintRenderer != null)
        {
            baseColor = _redTintRenderer.material.color;
        }

        if(_targetShield != null)
        {
            maxHealth = _targetShield.MaxHealth(ShieldIndex);
            currHealth = _targetShield.CurrentHealth(ShieldIndex);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_targetShield == null || _redTintRenderer == null)
            return;

        currHealth = _targetShield.CurrentHealth(ShieldIndex);

        float ratio = 1 - (currHealth / (float)maxHealth);
        baseColor.a = ratio;

        _redTintRenderer.material.color = baseColor;
    }
}
