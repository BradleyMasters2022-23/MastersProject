using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Storytelling/Character")]
public class Character : ScriptableObject
{
    public string characterName;
    public string[] spriteNames;
    public Sprite[] sprites;
    public Dictionary<string, Sprite> namedSprites = new Dictionary<string, Sprite>();

    private void Awake()
    {
        for(int i = 0; i < spriteNames.Length; i++)
        {
            namedSprites.Add(spriteNames[i], sprites[i]);
        }
    }
}
