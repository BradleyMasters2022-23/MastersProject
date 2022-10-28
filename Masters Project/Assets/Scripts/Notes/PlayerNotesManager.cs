using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNotesManager : MonoBehaviour {
    public bool[] notesCompleted;
    public NoteObject[] playerNotes;
    public static PlayerNotesManager instance;

    private void Awake() {
      if(instance == null) {
          instance = this;
          DontDestroyOnLoad(this.gameObject);
      } else {
          Destroy(this.gameObject);
      }
      notesCompleted = new bool[AllNotesManager.instance.notes.Count];
      playerNotes = new NoteObject[AllNotesManager.instance.notes.Count];
    }

    private void Start() {
      UpdateNotes();
    }

    private void UpdateNotes() {
      // loop through all notes
      foreach(NoteObject note in AllNotesManager.instance.notes) {
        // loop through all fragments in note
        for(int i = 0; i < note.fragments.Length; i++) {
          // if note is found
          if(note.fragments[i].found) {
            // make sure fragment is removed from lostFragments
            if(note.GetLostFragments().Contains(note.fragments[i])) {
              note.GetLostFragments().Remove(note.fragments[i]);
            }
            // update fragmentsFound
            note.GetFragmentsFound()[i] = true;
          }
        }

        // if all fragments found update playerNotes and notesComplete to reflect that
        if(note.AllFragmentsFound()) {
          playerNotes[note.ID] = note;
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
