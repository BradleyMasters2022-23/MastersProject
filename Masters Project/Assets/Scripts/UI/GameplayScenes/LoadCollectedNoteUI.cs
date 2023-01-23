using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LoadCollectedNoteUI : MonoBehaviour
{
    private List<NoteObject> collectedNotes;

    [SerializeField] private UnpackNoteUI[] options;

    [SerializeField] private SmartExpandContent contentDrawer;

    private void OnEnable()
    {
        if (PlayerNotesManager.instance == null)
            return;

        collectedNotes = new List<NoteObject>(PlayerNotesManager.instance.GetCollectedNotes());
        PopulateList();
    }

    /// <summary>
    /// Populate the list with collected notes, disable the rest
    /// </summary>
    private void PopulateList()
    {
        if (collectedNotes.Count > options.Length)
        {
            Debug.LogError("[LoadCollectedUpgradesUI] Warning! there are too many upgrades for the UI to load!" +
                "Increase the buffer for the options available!");
            return;
        }

        for(int i = 0; i < collectedNotes.Count; i++)
        {
            options[i].InitializeNoteUI(collectedNotes[i]);
            options[i].gameObject.SetActive(true);
        }

        for(int i = collectedNotes.Count; i < options.Length;i++)
        {
            options[i].ClearUI();
            options[i].gameObject.SetActive(false);
        }

        contentDrawer.CalculateHeight();
    }

    private void OnDisable()
    {
        // Unload all of the note stuff
        for (int i = 0; i < options.Length; i++)
        {
            options[i].ClearUI();
            options[i].gameObject.SetActive(false);
        }
    }

}
    
