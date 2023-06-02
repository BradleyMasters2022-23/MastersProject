/* ================================================================================================
 * Author - Ben Schuster   
 * Date Created - May 31th, 2023
 * Last Edited - May 31th, 2023 by Ben Schuster
 * Description - Save data and functions for notes and fragment system
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSaveData
{
    /// <summary>
    /// Dictionary of notes and fragments
    /// KEY - Note ID
    /// VALUE - List of all fragment IDs contained for that note
    /// </summary>
    public Dictionary<int, List<int>> collectedNotes;

    public NoteSaveData() 
    {
        //allNotes = new Dictionary<int, List<int>>();
        collectedNotes= new Dictionary<int, List<int>>();
    }

    /// <summary>
    /// Check whether a fragment has already been collected
    /// </summary>
    /// <param name="fragment">Fragment to check</param>
    /// <returns>Whether the fragment has already been collected</returns>
    public bool FragmentCollected(Fragment fragment)
    {
        // Check if a note has begun to be collected
        if (collectedNotes.ContainsKey(fragment.noteID))
        {
            return collectedNotes[fragment.noteID].Contains(fragment.fragmentID);
        }
        // If the note hasen't been found, then it has not been found
        else
        {
            return false;
        }   
    }

    public bool NoteStarted(NoteObject note)
    {
        return collectedNotes.ContainsKey(note.ID);
    }

    /// <summary>
    /// Check whether a note has been completed
    /// </summary>
    /// <param name="note">Note to check</param>
    /// <returns>Whether that note has all notes</returns>
    public bool NoteCompleted(NoteObject note)
    {
        // Check if a note has been completed
        if (collectedNotes.ContainsKey(note.ID))
        {
            return (note.fragments.Count == collectedNotes[note.ID].Count);
        }

        // if note not even started, then its not completed
        return false;
    }

    /// <summary>
    /// Add a fragment to be saved to the collected pool
    /// </summary>
    /// <param name="newFragment">New fragment to save</param>
    public void AddFragment(Fragment newFragment)
    {
        // If the note is already logged, and the fragment isnt, log the note
        if(collectedNotes.ContainsKey(newFragment.noteID)
            && !collectedNotes[newFragment.noteID].Contains(newFragment.fragmentID))
        {
            collectedNotes[newFragment.noteID].Add(newFragment.GetFragmentID());
        }
        // else if the note is not logged, log the note and the fragment
        else if(!collectedNotes.ContainsKey(newFragment.noteID))
        {
            List<int> list = new List<int>
            {
                newFragment.GetFragmentID()
            };

            collectedNotes.Add(newFragment.noteID, list);
        }
    }

    /// <summary>
    /// Print all IDs of notes and fragments read for completion sake
    /// </summary>
    public void PrintData()
    {
        Debug.Log($"All {collectedNotes.Count} notes started: ");
        
        foreach (var c in collectedNotes)
        {
            string temp = $" Note {c.Key} has the fragments : ";
            
            foreach(var v in c.Value)
            { temp += $"{v} | "; }

            Debug.Log(temp);
        }
    }
}
