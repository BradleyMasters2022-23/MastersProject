//ref: https://www.youtube.com/watch?v=YJLcanHcJxo&t=0s

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Line
{
    public Character character;
    public string SpriteName;

    [TextArea(2, 5)]
    public string text;

}

[CreateAssetMenu(menuName = "Storytelling/Conversation")]
public class Conversation : ScriptableObject
{
    public Character penny;
    public Character nonPennyCharacter;

    public Line[] lines;
}
