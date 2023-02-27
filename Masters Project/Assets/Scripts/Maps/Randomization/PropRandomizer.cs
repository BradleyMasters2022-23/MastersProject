/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - February 24th, 2022
 * Last Edited - February 27th, 2022 by Ben Schuster
 * Description - Concrete loading implementation for combat area segments.
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropRandomizer : MonoBehaviour, IRandomizer
{
    [SerializeField] private int minimumRoomNum;
    [SerializeField] private int maximumRoomNum;


    [SerializeField, Range(0, 100)] private float baseStayChance;
    [Tooltip("For ever depth of the room, how much the base chance is modified by." +
        "EX: If set to -10, then each room past minimum room number will decrease base chance by 10%")]
    [SerializeField, Range(-100, 100)] private float depthChanceModifier;

    /// <summary>
    /// Randomize whether or not this prop becomes active
    /// </summary>
    public void Randomize()
    {
        // Get depth of current run
        if (MapLoader.instance == null)
            return;
        int depth = MapLoader.instance.RoomDepth();

        // If outside bounds, always disable
        if(depth < minimumRoomNum || depth > maximumRoomNum)
        {
            gameObject.SetActive(false);
            return;
        }

        // Determine what chance to use
        float chance = Mathf.Clamp(baseStayChance + (depth - minimumRoomNum) * depthChanceModifier, 0, 100);

        // Determine of this should be off based on % chance
        if (Random.Range(0.05f, 100f) > chance)
        {
            gameObject.SetActive(false);
        }
    }
}
