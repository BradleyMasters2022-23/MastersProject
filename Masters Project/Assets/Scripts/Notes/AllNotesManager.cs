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
  public List<NoteObject> lostNotes = new List<NoteObject>();
  public static AllNotesManager instance;
  public FragmentContainer fragmentContainer;
  public GameObject container;
  public Transform fragmentSpawnPoint;

  private void Start() {
    if(instance == null) {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    } else {
        Destroy(this.gameObject);
    }

    foreach(NoteObject note in notes) {
      if(!note.completed) {
        lostNotes.Add(note);
      }
    }
  }

  public Fragment GetRandomLostFragment() {
    return notes[Random.Range(0, notes.Count)].GetRandomLostFragment();
  }

  public void SpawnFragment() {
    GameObject obj = Instantiate(container, fragmentSpawnPoint.transform.position, fragmentSpawnPoint.rotation);
    obj.GetComponent<FragmentContainer>().SetUp(GetRandomLostFragment());
    fragmentContainer = obj.GetComponent<FragmentContainer>();

    container.GetComponent<Collider>().enabled = true;
  }


}
