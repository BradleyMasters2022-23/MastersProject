/* ================================================================================================
 * Author - Soma Hannon
 * Date Created - October 21, 2022
 * Last Edited - October 31, 2022 by Soma Hannon
 * Description - Handles player's collected note objects.
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNotesManager : MonoBehaviour
{
    [Tooltip("Notes the player has completed.")]
    public List<NoteObject> playerNotes = new List<NoteObject>();
    public static PlayerNotesManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        UpdateNotes();
    }

    private void UpdateNotes()
    {
        // loop through all notes
        foreach(NoteObject note in AllNotesManager.instance.notes)
        {
            // loop through all fragments in note
            foreach(Fragment fragment in note.GetFragments())
            {
                // if fragment is found
                if(fragment.found)
                {
                    // if note is not in player's list of notes, add it
                    if(!playerNotes.Contains(note))
                    {
                        playerNotes.Add(note);
                    }
                }
            }

            note.UpdateNote();

            // if all fragments found update AllNotesManager to reflect that
            if(note.AllFragmentsFound())
            {
                AllNotesManager.instance.FindNote(note);
            }
        }
    }

    public void FindFragment(Fragment fragment)
    {
        fragment.found = true;
        Debug.Log(fragment.content);
        UpdateNotes();
    }

}
