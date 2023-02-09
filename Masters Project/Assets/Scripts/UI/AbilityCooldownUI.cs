using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AbilityCooldownUI : MonoBehaviour
{
    [SerializeField] private Ability targetRef;
    [SerializeField] private List<Slider> chargeUI;

    int chargeIndex;

    private void Awake()
    {
        if(targetRef == null || chargeUI.Count != targetRef.TotalCharges())
        {
            Debug.LogError($"Ability cooldown on {gameObject.name} was not set, deactivating!");
            enabled = false;
            return;
        }
    }

    private void LateUpdate()
    {
        chargeIndex = targetRef.CurrentCharges();
        
        for(int i = 0; i < targetRef.TotalCharges(); i++)
        {
            if(i < chargeIndex)
            {
                chargeUI[i].value = 1;
            }
            else if(i == chargeIndex)
            {
                chargeUI[i].value = targetRef.ChargeProgress();
            }
            else
            {
                chargeUI[i].value = 0;
            }
        }
        
    }

}
