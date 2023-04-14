using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class CrystalUIDisplay : MonoBehaviour
{
    private Crystal loadedCrystal;
    private int inventoryIndex;
    private Vector2 anchorPos = new Vector2(0, .5f);

    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Image displayImg;
    private CrystalSlotScreen parent;
    private RectTransform t;

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
        if (loadedCrystal != null)
            displayImg.sprite = loadedCrystal.stats[0].GetIcon();
    }

    public void LoadEmpty()
    {
        displayImg.sprite = defaultSprite;
    }

    public bool HasUpgrade()
    {
        return loadedCrystal!= null;
    }


    /// <summary>
    /// What happens when the upgrade is selected
    /// Move the icon slot adjacent to this
    /// </summary>
    public void SelectUpgrade()
    {
        parent.SelectEquipSlot(inventoryIndex);
    }

    public Vector2 GetAnchoredPos()
    {
        if (t != null)
            return t.position;
        else
            return Vector2.zero;
    }
}
