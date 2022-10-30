/* ================================================================================================
 * Author - Soma Hannon
 * Date Created - October 21st, 2022
 * Last Edited - October 21st, 2022 by Soma Hannon
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
  public List<Fragment> fragments;
  public bool completed;
}
