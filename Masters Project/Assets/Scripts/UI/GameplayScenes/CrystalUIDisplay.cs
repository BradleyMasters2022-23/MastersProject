using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using TMPro;

public class CrystalUIDisplay : MonoBehaviour
{
    /// <summary>
    /// The crystal loaded into this slot
    /// </summary>
    private Crystal loadedCrystal;
    /// <summary>
    /// The parent caller of this crystal slot.
    /// Only used in the crystal SELECT screen
    /// </summary>
    private CrystalSlotScreen parent;
    /// <summary>
    /// Reference to this object's rect transform
    /// </summary>
    private RectTransform t;
    /// <summary>
    /// The index of this object
    /// </summary>
    private int inventoryIndex;

    [Header("Setup")]
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Image displayImg;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI statsText;

    [SerializeField] private bool allowTrashing;
    [HideIf("@this.allowTrashing == false")]
    [SerializeField] private GameObject trashButton;

    private Dictionary<IStat, float> personalStatDict;

    /// <summary>
    /// Load the crystal into this object
    /// </summary>
    /// <param name="display">Reference to crystal screen</param>
    /// <param name="c">Crystal to display</param>
    /// <param name="index">Index of this crystal</param>
    public void LoadCrystal(CrystalSlotScreen display, Crystal c, int index)
    {
        t = GetComponent<RectTransform>();
        parent = display;
        loadedCrystal= c;
        inventoryIndex = index;

        if (loadedCrystal != null)
            UpdateUI();
        else
            LoadEmpty();
    }

    private void UpdateUI()
    {
        // Load empty just in case 
        if (loadedCrystal == null)
        {
            LoadEmpty();
            return;
        }

        if(displayImg != null)
        {
            displayImg.sprite = loadedCrystal.Icon();
            displayImg.color = loadedCrystal.GetColor();
            displayImg.enabled = true;
        }
            
        if(nameText != null)
            nameText.text = loadedCrystal.crystalName;

        if(statsText != null)
        {
            personalStatDict = loadedCrystal.GetStats();
            statsText.text = StatDictToString(personalStatDict);
        }

        if (trashButton != null)
            trashButton.gameObject.SetActive(allowTrashing);
    }

    public void LoadEmpty()
    {
        loadedCrystal= null;

        if (displayImg != null)
            displayImg.enabled = false;

        if (nameText != null)
            nameText.text = "Empty Slot";

        if (statsText != null)
            statsText.text = "";

        if(trashButton!= null)
            trashButton.gameObject.SetActive(false);
    }

    public bool HasUpgrade()
    {
        return loadedCrystal!= null;
    }


    /// <summary>
    /// What happens when the upgrade is selected
    /// Used to automatically keep track of indexing, regardless of where its located in list
    /// </summary>
    public void SelectUpgrade()
    {
        if(parent != null)
            parent.SelectEquipSlot(inventoryIndex);
    }

    public Vector2 GetAnchoredPos()
    {
        if (t == null)
            t = GetComponent<RectTransform>();

        return t.position;
    }

    public Dictionary<IStat, float> GetStatDict()
    {
        return personalStatDict;
    }

    /// <summary>
    /// Convert a dictionary of stats and floats into a string and send it to a designated text box
    /// Do not use for the complete stat screen, that needs different functionality
    /// </summary>
    /// <param name="stats">Dictionary of stats to display</param>
    /// <returns>String representation of the new dictionary, with color rich text added</returns>
    public string StatDictToString(Dictionary<IStat, float> stats)
    {
        string displayString = "";
        foreach (KeyValuePair<IStat, float> val in stats)
        {
            string colorHex;
            string startChar = "";
            if (val.Value >= 0)
            {
                startChar = "+";
                colorHex = CrystalManager.instance.PositiveTextHex();
            }
            else
            {
                colorHex = CrystalManager.instance.NegativeTextHex();
            }

            displayString += "<color=#" + colorHex + ">" + startChar + val.Value.ToString("0.0") + " " + val.Key.GetStatText() + "</color>";
            displayString += "\n";
        }

        return displayString;
    }
}
