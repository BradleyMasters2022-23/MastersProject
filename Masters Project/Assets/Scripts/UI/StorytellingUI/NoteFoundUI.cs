using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;


public class NoteFoundUI : MonoBehaviour
{
    private EventSystem eventSystem;
    private Fragment displayFrag;
    private NoteObject note;
    private FragmentInteract caller;

    [Tooltip("Reference to the title textbox")]
    [SerializeField] private TextMeshProUGUI title;
    [Tooltip("Reference to the description textbox")]
    [SerializeField] private TextMeshProUGUI content;

    public void LoadFragment(FragmentInteract f)
    {
        displayFrag = f.GetFragment();
        note = f.GetNote();
        caller = f;

        // Load in data. Backup error loading
        if (note != null)
        {
            this.title.text = note.displayName;
        }
        else
        {
            this.title.text = "#ERR0R#";
        }
        if(f != null)
        {
            this.content.text = displayFrag.content;
        }
        else
        {
            this.content.text = "#ERROR: DATA CORRUPTED#";
        }
        
    }

    public void OpenScreen()
    {
        if (eventSystem == null)
        {
            eventSystem = EventSystem.current;
        }

        GameManager.instance.ChangeState(GameManager.States.GAMEMENU);
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Close the screen, change state
    /// </summary>
    public void CloseScreen()
    {
        if(caller != null)
        {
            caller.DestroyFrag();
        }
        caller = null;
        GameManager.instance.CloseTopMenu();
    }

}
