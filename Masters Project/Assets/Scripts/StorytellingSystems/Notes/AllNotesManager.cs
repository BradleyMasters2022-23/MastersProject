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
using Sirenix.OdinInspector;

public class AllNotesManager : SaveDataContainer
{
    [Tooltip("All notes in the game to reference. Some may be handled independently.")]
    [SerializeField] private List<NoteObject> allNotes;

    [Tooltip("All notes in the game that can be found via random drops, whether found or not")]
    [SerializeField] private List<NoteObject> worldNoteDrops;

    [Tooltip("All notes currently able to be used in new fragments")]
    [SerializeField, ReadOnly, HideInEditorMode] private List<NoteObject> worldDropPool;

    /// <summary>
    /// Save data for notes 
    /// </summary>
    private NoteSaveData saveData;
    /// <summary>
    /// File name for the save data
    /// </summary>
    private const string saveDataName = "noteSaveData";

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
            return;
        }

        // Load data. If no data, then create fresh set
        saveData = DataManager.instance.Load<NoteSaveData>(saveDataName);
        if (saveData == null)
            saveData = new NoteSaveData();
    }

    /// <summary>
    /// Load the pool of notes using save data as a reference
    /// </summary>
    private void Start()
    {
        // Prepare the pool of notes to use in the world
        foreach (NoteObject note in worldNoteDrops)
        {
            // If a note is not yet complete, add it to the pool
            if(!saveData.NoteCompleted(note) && !worldDropPool.Contains(note))
            {
                worldDropPool.Add(note);
            }
        }

        // debug checking
        // Debug.Log($"Lost notes size {worldDropPool.Count}");
        UpdateNotes();
    }

    /// <summary>
    /// returns a random NoteObject from lostNotes
    /// </summary>
    public NoteObject GetRandomLostNote()
    {
        // Make sure there are world drops available
        if (worldDropPool == null || worldDropPool.Count <= 0)
            return null;

        // Return a random note
        int ran = Random.Range(0, worldDropPool.Count);
        return worldDropPool[ran];
           
    }

    /// <summary>
    /// If a note is completed, remove it from the selection pool
    /// </summary>
    /// <param name="note"> note to be removed from lostNotes</param>
    public void FindNote(NoteObject note)
    {
        if (worldDropPool.Contains(note) && saveData.NoteCompleted(note)) 
        {
            worldDropPool.Remove(note);
            if (worldDropPool.Count <= 0)
                Debug.Log("The note drop pool is now empty!");
        }
    }

    public bool NoteFindable()
    {
        return worldDropPool.Count > 0;
    }

    public NoteObject GetNote(int index)
    {
        if (allNotes.Count > index)
            return allNotes[index];
        else
            return null;
    }

    public bool FragmentCollected(Fragment frag)
    {
        return saveData.FragmentCollected(frag);
    }


    /// <summary>
    /// Checks whether a note is completed
    /// </summary>
    /// <param name="note">Note to check</param>
    /// <returns>Whether the note is complete</returns>
    public bool CheckNoteComplete(NoteObject note)
    {
        return saveData.NoteCompleted(note);
    }

    /// <summary>
    /// Get a list of all collected notes. A collected note is defined by a note with atleast 1 fragment found
    /// </summary>
    /// <returns>New list of all collected notes</returns>
    public List<NoteObject> GetCollectedNotes()
    {
        // prepare a new container
        List<NoteObject> foundNotes = new List<NoteObject>();

        // Loop through each note and see if it's found in save data 
        foreach(var data in allNotes)
        {
            if (saveData.NoteStarted(data))
                foundNotes.Add(data);
        }

        // return list of all found notes
        return foundNotes;
    }

    public void FragmentFound(Fragment frag)
    {
        saveData.AddFragment(frag);
        UpdateNotes();
    }

    private void UpdateNotes()
    {
        // loop through all notes
        foreach (NoteObject note in allNotes)
        {
            // Tell each note to check its own list to confirm
            note.UpdateNote(saveData);

            // if all fragments found, mark it as complete
            if (saveData.NoteCompleted(note))
            {
                FindNote(note);
            }
        }

        // Save the data 
        DataManager.instance.Save(saveDataName, saveData);
        //saveData.PrintData();
    }

    public override void ResetData()
    {
        saveData = new NoteSaveData();
        UpdateNotes();
    }
}
