using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Storytelling/Character")]
public class Character : ScriptableObject
{
    public string characterName;
    public string[] spriteNames;
    public Sprite[] sprites;

}
