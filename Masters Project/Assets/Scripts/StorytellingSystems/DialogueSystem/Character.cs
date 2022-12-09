using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay/Character")]
public class Character : ScriptableObject
{
    public string characterName;
    public Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
}
