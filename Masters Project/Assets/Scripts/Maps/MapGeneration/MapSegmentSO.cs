using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Map Segment", menuName = "Gameplay/Map Segment")]
public class MapSegmentSO : ScriptableObject
{
    //public enum MapSegmentType
    //{
    //    Room,
    //    Hallway,
    //    FinalRoom
    //}

    //[Tooltip("Type of map section this is")]
    //[EnumToggleButtons, PropertySpace(8, 8)]
    //public MapSegmentType segmentType;

    //[Tooltip("The prefab containing this map section")]
    //[PropertySpace(0, 8), AssetsOnly, PreviewField( Alignment = ObjectFieldAlignment.Center, Height = 200)]
    //public GameObject segmentPrefab;

    [Tooltip("Name of the scene this level has")]
    public string sceneName;

    //[ShowIf("@this.segmentType == MapSegmentType.Room")]
    [Tooltip("The minimum difficulty level this room can be used in")]
    public int minDifficulty = 0;
    //[ShowIf("@this.segmentType == MapSegmentType.Room")]
    [Tooltip("The maximum difficulty level this room can be used in")]
    public int maxDifficulty = 999;

    //[ShowIf("@this.segmentType == MapSegmentType.Room")]
    //[Tooltip("Potential encounters this room can have")]
    //public EncounterSO[] potentialEncounters;

    /// <summary>
    /// Check if the difficulty level can be used
    /// </summary>
    /// <param name="difficulty">current difficulty level</param>
    /// <returns>whether this room can be used given the current difficulty</returns>
    public bool WithinDifficulty(int difficulty)
    {
        return (difficulty <= maxDifficulty && difficulty >= minDifficulty);
    }
}
