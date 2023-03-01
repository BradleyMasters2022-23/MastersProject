/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - March 1st, 2022
 * Last Edited - March 1st, 2022 by Ben Schuster
 * Description - Generic weighted list that can be used for various systems later. 
 * Automatically handles the action of calculating probability and choosing an option
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class WeightedChoice<T>
{
    public T option;
    [Range(1, 1000)] public int weight = 1;
    
    [HideIf("@this.estimateProbability == 0"), ReadOnly] 
    public float estimateProbability = 0;
}


[System.Serializable]
public class GenericWeightedList<T>
{
    [ListDrawerSettings(Expanded = true, ShowItemCount = true)]
    [OnInspectorGUI("EstimateProbability")]
    [TableList(AlwaysExpanded = true, CellPadding = 7, HideToolbar = false)] 
    public List<WeightedChoice<T>> weightedList;

    private float[] odds;

    public GenericWeightedList()
    {
        weightedList = new List<WeightedChoice<T>>();
    }

    public T Pull()
    {
        // Skip randomness if no or one option
        if (weightedList.Count == 0)
            return default(T);
        else if (weightedList.Count == 1)
            return weightedList[0].option;

        EstimateProbability();

        // Roll the die, see which wins
        float ran = Random.Range(0f, 1f);
        float temp = 0;
        for(int i = 0; i < odds.Length; i++)
        {
            temp += odds[i];

            if(temp >= ran)
            {
                return weightedList[i].option;
            }
        }

        // Incase anything fails, return the last item. 
        // Above logic should always return something however. 
        return weightedList[weightedList.Count - 1].option;
    }

    public void EstimateProbability()
    {
        // Get sum of all weights in list
        int total = 0;
        foreach (WeightedChoice<T> choice in weightedList)
            total += choice.weight;

        // Determine chances based on weights 
        float[] odds = new float[weightedList.Count];
        for (int i = 0; i < weightedList.Count; i++)
        {
            odds[i] = (float)weightedList[i].weight / total;
            weightedList[i].estimateProbability = odds[i] * 100;
        }
    }
}
