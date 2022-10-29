using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNotesManager : MonoBehaviour {
    public bool[] notesCompleted;
    public List<NoteObject> playerNotes = new List<NoteObject>();
    public static PlayerNotesManager instance;

    private void Awake() {
      if(instance == null) {
          instance = this;
          DontDestroyOnLoad(this.gameObject);
      } else {
          Destroy(this.gameObject);
      }
    }

    private void Start() {
      notesCompleted = new bool[AllNotesManager.instance.notes.Count];

      UpdateNotes();
    }

    private void UpdateNotes() {
      // loop through all notes
      foreach(NoteObject note in AllNotesManager.instance.notes) {
        // loop through all fragments in note
        for(int i = 0; i < note.fragments.Length; i++) {
          // if fragment is found
          if(note.fragments[i].found) {
            // if note is not in player's list of notes, add it
            if(!playerNotes.Contains(note)) {
              playerNotes.Add(note);
            }
            // make sure fragment is removed from lostFragments
            if(note.GetLostFragments().Contains(note.fragments[i])) {
              note.GetLostFragments().Remove(note.fragments[i]);
            }
            // update fragmentsFound
            note.GetFragmentsFound()[i] = true;
          }
        }

        note.UpdateNote();

        // if all fragments found update playerNotes and notesComplete to reflect that
        if(note.AllFragmentsFound()) {
          notesCompleted[note.ID] = true;
          AllNotesManager.instance.FindNote(note);
        } else {
          notesCompleted[note.ID] = false;
        }
      }
    }

    public void FindFragment(Fragment fragment) {
      fragment.found = true;
      Debug.Log(fragment.content);
      UpdateNotes();
    }

}
