/* ================================================================================================
 * Author - Soma Hannon
 * Date Created - October 21, 2022
 * Last Edited - October 31, 2022 by Soma Hannon
 * Description - Base note object.
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Storytelling/Note Data")]
public class NoteObject : ScriptableObject
{
    [Tooltip("ID of this note.")]
    public int ID;

    [Tooltip("Name of this note to be displayed.")]
    public string displayName;

    [Tooltip("List of fragments contained in this note.")]
    public List<Fragment> fragments = new List<Fragment>();

    /// <summary>
    /// list of all fragments player has not found
    /// </summary>
    private List<Fragment> lostFragments = new List<Fragment>();

    /// <summary>
    /// updates lostFragments and completed
    /// </summary>
    public void UpdateNote(NoteSaveData dataRef)
    {
        lostFragments.Clear();

        // If no data found, then mark all fragments as lost
        if(dataRef == null)
        {
            lostFragments = new List<Fragment>(fragments);
            return;
        }

        // If the save data says all data is complete, cancel early
        if (dataRef.NoteCompleted(this))
        {
            lostFragments.Clear();
            return;
        }
        // Otherwise, populate lost list with fragments not found
        else
        {
            foreach (Fragment fragment in fragments)
            {
                // if fragment is not found
                if (!dataRef.FragmentCollected(fragment))
                {
                    lostFragments.Add(fragment);
                }
            }
        }
    }

    /// <summary>
    /// returns list of fragments
    /// </summary>
    public List<Fragment> GetFragments()
    {
      return fragments;
    }

    /// <summary>
    /// returns a random fragment from lostFragments, but does not mark it as read
    /// </summary>
    public Fragment GetRandomLostFragment()
    {
        int randNum = Random.Range(0, lostFragments.Count);
        //Debug.Log("Lost frag size : " + lostFragments.Count + " | " + randNum);

        // if out of options, return nothing
        if (lostFragments.Count <= 0)
            return null;

        return lostFragments[randNum];
    }

    public bool FragmentFound(Fragment f)
    {
        //Debug.Log($"Checking fragment with note id {ID} | id {f.fragmentID}");
        return !lostFragments.Contains(f);
    }
}
