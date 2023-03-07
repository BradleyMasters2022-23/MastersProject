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

    [Tooltip("Whether or not this note has been completed.")]
    public bool completed;

    /// <summary>
    /// initial update to initialize lostFragments
    /// </summary>
    private void Awake()
    {
        UpdateNote();
    }

    /// <summary>
    /// updates lostFragments and completed
    /// </summary>
    public void UpdateNote()
    {
        foreach(Fragment fragment in fragments)
        {
            // if fragment is found
            if(fragment.found)
            {
                // if lostFragments contains the fragment, remove it
                if(lostFragments.Contains(fragment))
                {
                    lostFragments.Remove(fragment);
                }
            }
            // if fragment is not found
            else
            {
                // if lostFragments does not contain the fragment, add it
                if(!lostFragments.Contains(fragment))
                {
                    lostFragments.Add(fragment);
                }
            }
        }

        // if all fragments are found, mark note as completed
        if(AllFragmentsFound())
        {
          completed = true;
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
    /// returns a random fragment from lostFragments
    /// </summary>
    public Fragment GetRandomLostFragment()
    {
      return lostFragments[Random.Range(0, lostFragments.Count)];
    }

    /// <summary>
    /// checks all fragments and if all have been found, returns true
    /// </summary>
    public bool AllFragmentsFound()
    {
        foreach(Fragment fragment in fragments)
        {
            if(!fragment.found)
            {
                return false;
            }
        }

        return true;
    }
}
