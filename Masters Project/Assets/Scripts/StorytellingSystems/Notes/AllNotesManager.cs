/* ================================================================================================
 * Author - Soma Hannon
 * Date Created - October 21, 2022
 * Last Edited - October 31, 2022 by Soma Hannon
 * Description - Handles all note objects.
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllNotesManager : MonoBehaviour
{
    /// <summary>
    /// list of all extant notes
    /// </summary>
    public List<NoteObject> notes = new List<NoteObject>();

    /// <summary>
    /// list of all notes player has not completed
    /// </summary>
    private List<NoteObject> lostNotes = new List<NoteObject>();

    /// <summary>
    /// object to call this class easily
    /// </summary>
    public static AllNotesManager instance;

    /// <summary>
    /// sets up the manager
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start() {
        // adds all NoteObjects in notes to lostNotes on first load
        foreach (NoteObject note in notes)
        {
            foreach(Fragment fragment in note.fragments)
            {
                fragment.found = false;
            }

            if (!note.AllFragmentsFound() && !lostNotes.Contains(note))
            {
                //Debug.Log($"Adding to lost notes {note.name} | {note.GetAllLostFragments().Count}");
                lostNotes.Add(note);
            }
        }
        //Debug.Log($"Lost notes size {lostNotes.Count}");
    }

    /// <summary>
    /// returns a random NoteObject from lostNotes
    /// </summary>
    public NoteObject GetRandomLostNote()
    {
        int ran = Random.Range(0, notes.Count);
        //Debug.Log($"Lost notes size {lostNotes.Count} | Random Num {ran}");
        
        if (lostNotes != null && lostNotes.Count > 0 && ran < lostNotes.Count)
            return lostNotes[ran];
        else
        {
            //Debug.Log("Potential break detected in AllNoteManager, but prevented!");
            return null;
        }
            
    }

    /// <summary>
    /// called when player collects a note fragment; removes NoteObject from lostNotes if it's the
    /// first fragment from that note collected
    /// </summary>
    /// <param name="note"> note to be removed from lostNotes</param>
    public void FindNote(NoteObject note)
    {
        if (lostNotes.Contains(note)) {
            lostNotes.Remove(note);
        }
        return;
    }

    public bool NoteFindable()
    {
        
        if (lostNotes.Count == 0)
        {
            return false;
        }

        return true;
    }

    public NoteObject GetNote(int index)
    {
        return notes[index];
    }

    public List<NoteObject> GetNotes()
    {
        return notes;
    }
}
