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

    [Tooltip("Render probe map to use in the portal to display this room")]
    public Cubemap portalViewMat;
    [Tooltip("What to set the probe intensity (brightness) as")]
    public float probeIntensityLevel;

    [Tooltip("Does this room have a depth requirement")]
    public bool depthRequirement = true;

    [ShowIf("@this.depthRequirement")]
    [Tooltip("The minimum difficulty level this room can be used in")]
    public int minDifficulty = 0;
    [ShowIf("@this.depthRequirement")]
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
        return !depthRequirement || (difficulty <= maxDifficulty && difficulty >= minDifficulty);
    }
}
