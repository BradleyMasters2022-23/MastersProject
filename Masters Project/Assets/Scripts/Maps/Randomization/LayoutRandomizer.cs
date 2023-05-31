/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - February 27th, 2022
 * Last Edited - February 27th, 2022 by Ben Schuster
 * Description - Concrete randomizer that determines atleast one of its layouts to be used, if within range
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public struct Layout
{
    [Tooltip("Root gameobject of the entire layout")]
    public GameObject layoutRoot;
    [Tooltip("Minimum room depth required for this layout to be used")]
    public int minRoomDepth;
    [Tooltip("Maximum room depth required for this layout to be used")]
    public int maxRoomDepth;

    [Tooltip("Weight of this chance compared to others. Default is 1.")]
    [Range(1, 10)] public int probabilityWeight;
    [Tooltip("Amount weight is adjusted by, for every room past its minimum depth requirement")]
    [Range(-10, 10)] public int weightDepthMod;

    /// <summary>
    /// Get the current weight based on depth. Preclamped.
    /// </summary>
    /// <param name="depth">Current room depth</param>
    /// <returns>Layout weight adjusted for depth</returns>
    public int ModdedWeight(int depth)
    {
        return Mathf.Clamp(probabilityWeight + (weightDepthMod * (depth - minRoomDepth)), 1, 10);
    }
}


public class LayoutRandomizer : MonoBehaviour, IRandomizer
{
    [Tooltip("All potential layouts. Organize the rarest options at the top.")]
    [SerializeField] private Layout[] layouts;

    [Tooltip("Enable if you are testing without the maploader present.")]
    [SerializeField] private bool testing;
    [HideIf("@this.testing == false")]
    [Tooltip("The current run depth. Must be set manually in testing but automatically set otherwise.")]
    [SerializeField] private int depth;

    protected int chosenIndex = -1;

    protected bool randomized = false;

    protected virtual void Start()
    {
        // disable layouts to begin with
        foreach(Layout layout in layouts)
        {
            layout.layoutRoot.SetActive(false);
        }

        // If testing, call randomize in start
        if (testing)
            Randomize();
    }

    /// <summary>
    /// Randomly select a layout based on what can be used and weights
    /// </summary>
    public void Randomize()
    {
        // Get depth of current run
        if (!testing && MapLoader.instance != null)
        {
            depth = MapLoader.instance.PortalDepth();
        }

        // Determine which rooms can be used
        List<Layout> usableLayouts = new List<Layout>();
        int totalWeight = 0;
        foreach(Layout layout in layouts)
        {
            // make sure the layout is initialized correctly
            if(layout.layoutRoot == null) 
                continue;

            // only use ones that are within depth limit
            if(depth >= layout.minRoomDepth && depth <= layout.maxRoomDepth)
            {
                usableLayouts.Add(layout);
                totalWeight += layout.ModdedWeight(depth);
            }
        }

        // If no usable layouts, exit
        if (usableLayouts.Count <= 0)
        {
            chosenIndex = -1;
            return;
        }
        // if only one usable layout, just set it active and skip randomization calculations
        else if (usableLayouts.Count == 1)
        {
            chosenIndex = 0;
            usableLayouts[0].layoutRoot.SetActive(true);
            return;
        }

        // Determine chances based on weights
        float[] odds = new float[usableLayouts.Count];
        for(int i = 0; i < odds.Length; i++)
        {
            odds[i] = (float)usableLayouts[i].ModdedWeight(depth) / totalWeight;
        }

        randomized = true;

        // determine which outcome based on chance
        float ran = Random.Range(0f, 1f);
        // temp will add current value and see if its greater than ran
        float temp = 0;
        for (int i = 0; i < odds.Length; i++)
        {
            temp += odds[i];

            // If temp is greater than ran, then select this item and activate it
            if(temp >= ran)
            {
                chosenIndex = i;
                usableLayouts[i].layoutRoot.SetActive(true);
                return;
            }
        }
    }
}
