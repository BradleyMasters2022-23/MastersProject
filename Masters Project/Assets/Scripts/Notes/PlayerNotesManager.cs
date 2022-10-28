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
      // populates notesFound with bools telling whether or not a particular note has been completed

      for(int i = 0; i < notesCompleted.Length; i++) {
        if(AllNotesManager.instance.notes[i].completed) {
          notesCompleted[i] = true;
        } else {
          notesCompleted[i] = false;
        }
      }
    }

    public void FindFragment(Fragment fragment) {
      fragment.found = true;
      UpdateNotes();
    }

}
