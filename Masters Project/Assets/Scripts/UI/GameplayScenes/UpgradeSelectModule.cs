using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class UpgradeSelectModule : MonoBehaviour
{
    [HideInInspector] public UpgradeObject upgradeData;

    [Tooltip("Reference to the title textbox")]
    [SerializeField] private TextMeshProUGUI titleText;
    [Tooltip("Reference to the description textbox")]
    [SerializeField] private TextMeshProUGUI descriptionText;
    [Tooltip("Reference to the icon img")]
    [SerializeField] private Image upgradeIcon;

    public void InitializeUIElement(UpgradeObject o)
    {
        upgradeData = o;

        titleText.text = upgradeData.displayName;
        descriptionText.text = upgradeData.displayDesc;
        upgradeIcon.sprite = upgradeData.displayIcon;

        gameObject.SetActive(true);
    }

    public void ClearUI()
    {
        upgradeData = null;

        titleText.text = "";
        descriptionText.text = "";
        upgradeIcon.sprite = null;

        gameObject.SetActive(false);
    }
}
