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
  /// <summary>
  /// notes player has completed
  /// </summary>
    [SerializeField] private List<NoteObject> playerNotes = new List<NoteObject>();

    /// <summary>
    /// call this class easily
    /// </summary>
    public static PlayerNotesManager instance;

    /// <summary>
    /// initializes instance
    /// </summary>
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

    /// <summary>
    /// initial update
    /// </summary>
    private void Start()
    {
        UpdateNotes();
    }

    /// <summary>
    /// updates each note's fragments, each note, and the list of all notes
    /// </summary>
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

    /// <summary>
    /// call when player finds a fragment; sets fragment to found and updates
    /// </summary>
    public void FindFragment(Fragment fragment)
    {
        fragment.found = true;
        Debug.Log("Fragment number " + fragment.fragmentID + " of note number " + fragment.noteID + " was collected!");
        UpdateNotes();
    }

    /// <summary>
    /// Get currently collected player notes
    /// </summary>
    /// <returns></returns>
    public List<NoteObject> GetCollectedNotes()
    {
        return playerNotes;
    }
}
