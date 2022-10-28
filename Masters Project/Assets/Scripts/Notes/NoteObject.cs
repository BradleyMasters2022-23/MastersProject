/* ================================================================================================
 * Author - Soma Hannon
 * Date Created - October 21, 2022
 * Last Edited - October 28, 2022 by Soma Hannon
 * Description - Base note object.
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay/Note Data")]
public class NoteObject : ScriptableObject {
  public int ID;
  public string displayName;
  public int numFragments;
  public Fragment[] fragments;
  public List<Fragment> lostFragments;
  public bool[] fragmentsFound;
  public bool completed;

  private void Awake() {
    fragments = new Fragment[numFragments];
    fragmentsFound = new bool[numFragments];

    foreach(Fragment fragment in fragments) {
      if(!fragment.found) {
        lostFragments.Add(fragment);
      }
    }
  }

  private void Start() {
    UpdateNote();
  }

  private void UpdateNote() {
    for(int i = 0; i < fragmentsFound.Length; i++) {
      if(fragments[i].found) {
        fragmentsFound[i] = true;
      } else {
        fragmentsFound[i] = false;
      }
    }
  }

  public Fragment[] GetFragments() {
    return fragments;
  }

  public bool[] GetFragmentsFound() {
    return fragmentsFound;
  }

  public Fragment GetFragment(int index) {
    return fragments[index];
  }

  public Fragment GetRandomLostFragment() {
    return lostFragments[Random.Range(0, lostFragments.Count)];
  }
}
