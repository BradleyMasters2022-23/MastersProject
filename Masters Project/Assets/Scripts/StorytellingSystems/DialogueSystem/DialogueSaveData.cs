/* ================================================================================================
 * Author - Ben Schuster   
 * Date Created - May 5th, 2023
 * Last Edited - May 5th, 2023 by Ben Schuster
 * Description - Save data and functions for dialogue system
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogueSaveData
{
    /// <summary>
    /// Dictionary of conversations and runs
    /// 
    /// KEY - Conversation ID. 
    /// Use ID's as otherwise, we need to write our own JSON converter for SO's
    /// 
    /// VALUE - Number of runs SINCE the conversation was read
    /// </summary>
    public Dictionary<int, int> readConversations;

    public DialogueSaveData() 
    {
        readConversations= new Dictionary<int, int>();
    }

    public void ConversationRead(Conversation c)
    {
        // Add to list of read conversations
        if(!readConversations.ContainsKey(c.ID))
        {
            readConversations.Add(c.ID, 0);
        }
    }

    /// <summary>
    /// Increment all conversation runs
    /// </summary>
    public void IncrementRuns()
    {
        foreach (var c in readConversations.ToArray())
            readConversations[c.Key]++;
    }

    /// <summary>
    /// Check whether or not this conversation has been read
    /// </summary>
    /// <param name="c">conversation to check</param>
    /// <returns>WHether this conversation has already been read</returns>
    public bool AlreadyRead(Conversation c)
    {
        return readConversations.ContainsKey(c.ID);
    }

    /// <summary>
    /// Check whether dependencies have been satisfied yet
    /// </summary>
    /// <param name="c">Conversation to check</param>
    /// <returns>Whether or not dependencies have been satisfied</returns>
    public bool CheckDependencies(Conversation c)
    {
        //Debug.Log($"Checking D's on conv {c.ID}");
        // If no natural dependencies, check number of runs
        if (c.dependencies.Length <= 0)
        {
            //Debug.Log($"No dependencies, comparing {c.runReq} <= {GlobalStatsManager.data.runsAttempted}");
            return GlobalStatsManager.data.runsAttempted >= c.runReq;
        }
        // otherwise, check if the number of runs since all dependencies have been met
        else
        {
            foreach(Conversation reqConv in c.dependencies) 
            {
                // If a dependency has not been read yet OR number of runs since read conversation
                // has not been met yet, then the conversation's dependencies have NOT been met
                if (!readConversations.ContainsKey(reqConv.ID) || readConversations[reqConv.ID] < c.runReq)
                {
                    //Debug.Log($"{c.ID} cannot be used as it failed its dependency with {reqConv.ID}");
                    return false;
                }
            }

            // If full loop was completed, then dependencies have been met
            return true;
        }
    }

    /// <summary>
    /// See all read conversations. Use for debugging.
    /// </summary>
    public void SeeAllReads()
    {
        Debug.Log($"All {readConversations.Count} conversations read: ");
        foreach (var c in readConversations)
        {
            Debug.Log($"Conv ID {c} | {c.Value} runs ago");
        }
    }
}
