/* ================================================================================================
 * Author - Soma Hannon
 * Date Created - October 21st, 2022
 * Last Edited - October 21st, 2022 by Soma Hannon
 * Description - Handles all note objects.
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllNotesManager : MonoBehaviour {
  public List<NoteObject> notes = new List<NoteObject>();
  private List<NoteObject> lostNotes = new List<NoteObject>();
  public static AllNotesManager instance;

  private void Start() {
    if(instance == null) {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    } else {
        Destroy(this.gameObject);
    }

    foreach(NoteObject note in notes) {
      if(!note.AllFragmentsFound()) {
        lostNotes.Add(note);
      }
    }
  }

  public NoteObject GetRandomLostNote() {
    return lostNotes[Random.Range(0, notes.Count)];
  }

  public void FindNote(NoteObject note) {
    if(lostNotes.Contains(note)) {
      lostNotes.Remove(note);
    }
    return;
  }


}
