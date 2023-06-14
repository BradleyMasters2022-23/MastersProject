using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Masters.UI;

public class CrystalSlotScreen : MonoBehaviour
{
    private CrystalInteract caller;
    /// <summary>
    /// The new crystal being offered
    /// </summary>
    private Crystal newCrystal;
    /// <summary>
    /// The currently selected/hovered crystal
    /// </summary>
    private Crystal selectedCrystal;
    private int selectedCrystalIndex = 0;

    // Currently empty, can be recommented to reimplement
    // [SerializeField] private Sprite blank;

    [SerializeField] private CrystalUIDisplay newCrystalDisplay;
    [SerializeField] private CrystalUIDisplay hoverCrystalDisplay;
    [SerializeField] private AllStatsDisplay pennysStats;


    [Tooltip("All inventory slots available to choose from")]
    [SerializeField] private CrystalUIDisplay[] allSlots;

    [SerializeField] private RectTransform newCrystalTransform;
    [SerializeField] private RectTransform trashDisplay;

    [SerializeField] private Animator animator;

    /// <summary>
    /// Reference to the global instance 
    /// </summary>
    private CrystalManager crystalManager;

    private bool init = false;

    /// <summary>
    /// Reference to the confirmation box
    /// </summary>
    private ConfirmationBox confirmBox;

    /// <summary>
    /// Open the screen, change state
    /// </summary>
    public void OpenScreen(CrystalInteract c)
    {
        caller = c;
        newCrystal = caller.GetCrystal();
        selectedCrystalIndex = 0;
        crystalManager = CrystalManager.instance;

        if(crystalManager == null)
        {
            Debug.LogError("Error! Tried to call open crystal screen, but theres no crystal manager!");
            return;
        }

        GameManager.instance.ChangeState(GameManager.States.GAMEMENU);
        gameObject.SetActive(true);

        animator.enabled = true;
        animator.ResetPlay();
        animator.SetTrigger("HoverStart");

        InitializeEquippedCrystals();
        DisplayNewCrystalStats();
    }

    private void DisplayPennyStats()
    {
        pennysStats.Clear();

        if (selectedCrystalIndex > -1)
            pennysStats.DetermineDifferences(newCrystal, crystalManager.GetEquippedCrystal(selectedCrystalIndex));
        else
            pennysStats.LoadEquippedStats();
    }

    private void InitializeEquippedCrystals()
    {
        gameObject.SetActive(true);

        pennysStats.LoadEquippedStats();

        for (int i = 0; i < crystalManager.MaxSlots(); i++)
        {
            // Load crystal function will automatically apply blank image if given a null crystal
            allSlots[i].LoadCrystal(this, crystalManager.GetEquippedCrystal(i), i);
        }

        bool found = false;
        // Move selection to first empty slot, if possible. Auto calls string funcs
        for (int i = 0; i < allSlots.Length; i++)
        {
            if (!allSlots[i].HasUpgrade())
            {
                found = true;
                allSlots[i].SelectUpgrade();
                break;
            }
        }
        // If no default slot found, set to first slot
        if(!found)
            allSlots[0].SelectUpgrade();

        // Load current new icon
        newCrystalDisplay.LoadCrystal(this, newCrystal, -1);

        init = true;
    }

    private void DisplayNewCrystalStats()
    {
        newCrystalDisplay.LoadCrystal(this, newCrystal, -1);
    }

    /// <summary>
    /// called by unity button
    /// </summary>
    public void SelectEquipSlot(int index)
    {
        // Mark old slot to not trash, just in case
        if (selectedCrystalIndex == -1)
            newCrystalDisplay.MarkForTrash(false);
        else
            allSlots[selectedCrystalIndex].MarkForTrash(false);

        selectedCrystalIndex = index;

        // Only do after init. otherwise, unity screen loading is weird and its wrong
        if(init)
            newCrystalTransform.position = allSlots[index].GetAnchoredPos();

        if (crystalManager.GetEquippedCrystal(index) != null)
        {
            selectedCrystal = crystalManager.GetEquippedCrystal(index);
            DisplaySelectedCrystal();
            allSlots[selectedCrystalIndex].MarkForTrash(true);
            animator.SetInteger("DestroyTarget", 1);
        } 
        else
        {
            animator.SetInteger("DestroyTarget", 0);
            hoverCrystalDisplay.LoadEmpty();
        }

        DisplayPennyStats();
    }

    /// <summary>
    /// displays a selected currently-equipped crystal
    /// </summary>
    private void DisplaySelectedCrystal()
    {
        hoverCrystalDisplay.LoadCrystal(this, selectedCrystal, selectedCrystalIndex);
    }

    /// <summary>
    /// Set it to trash
    /// </summary>
    public void Trash()
    {
        newCrystalTransform.position = trashDisplay.position;

        // clear trash mark status from old, apply to current
        if(selectedCrystalIndex != -1)
            allSlots[selectedCrystalIndex].MarkForTrash(false);
        newCrystalDisplay.MarkForTrash(true);

        selectedCrystalIndex = -1;
        hoverCrystalDisplay.LoadEmpty();

        animator.SetInteger("DestroyTarget", -1);

        DisplayPennyStats();
    }

    /// <summary>
    /// Try applying changes, send warning first if appropriate
    /// </summary>
    public void RequestApplyChanges()
    {
        // Try to get box if null
        if(confirmBox == null)
            confirmBox = FindObjectOfType<ConfirmationBox>(true);

        // If failed to get box, or no crystal will be replaced, just apply the new change
        if (confirmBox == null)
        {
            ApplyChanges();
            return;
        }
            
        // Build the string based on trashing or replacing a crystal
        string txt = "";
        // Prompt for TRASHING NEW CRYSTAL
        if (selectedCrystalIndex == -1)
        {
            txt = $"Discard new crystal <b>{newCrystal.crystalName}?</b>";
        }
        // Prompt for REPLACING A CRYSTAL
        else if(crystalManager.CrystalEquipped(selectedCrystalIndex))
        {
            txt = $"Replace <b>{selectedCrystal.crystalName}?</b>";
        }
        // If not replacing 
        else
        {
            ApplyChanges();
            return;
        }

        // Send the request
        confirmBox.RequestConfirmation(ApplyChanges, txt);
    }

    /// <summary>
    /// Apply changes, start equip animation
    /// </summary>
    private void ApplyChanges()
    {
        if(selectedCrystalIndex >= 0)
        {
            GlobalStatsManager.data.crystalsCollected++;
            crystalManager.LoadCrystal(newCrystal, selectedCrystalIndex);
        }
        caller.BegoneCrystalInteract();

        animator.SetTrigger("SlotCrystal");
        // disable UI input during equip animation to prevent bugs
        GameManager.controls.UI.Disable();

        //GetComponent<GameObjectMenu>().CloseButton();
    }
}
