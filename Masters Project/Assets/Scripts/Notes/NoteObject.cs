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
  private List<Fragment> lostFragments = new List<Fragment>();
  private bool[] fragmentsFound;

  private void Awake() {
    fragments = new Fragment[numFragments];
    fragmentsFound = new bool[numFragments];
  }

  private void Start() {
    UpdateNote();
  }

  public void UpdateNote() {
    for(int i = 0; i < fragmentsFound.Length; i++) {
      if(fragments[i].found) {
        fragmentsFound[i] = true;
      } else {
        fragmentsFound[i] = false;
        if(!lostFragments.Contains(fragments[i])) {
          lostFragments.Add(fragments[i]);
        }
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

  public bool AllFragmentsFound() {
    foreach(Fragment fragment in fragments) {
      if(!fragment.found) {
        return false;
      }
    }

    return true;
  }

  public List<Fragment> GetLostFragments() {
    return lostFragments;
  }
}
