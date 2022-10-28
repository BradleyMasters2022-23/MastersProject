using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay/Fragment Data")]
public class Fragment : ScriptableObject {
  public int fragmentID;
  public int noteID;
  public string content;
  public bool found;
}
