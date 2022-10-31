/* ================================================================================================
 * Author - Soma Hannon
 * Date Created - October 21, 2022
 * Last Edited - October 28, 2022 by Soma Hannon
 * Description - Base note object.
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay/Note Data")]
public class NoteObject : ScriptableObject {
    public int ID;
    public string displayName;
    public int numFragments;
    public List<Fragment> fragments = new List<Fragment>();
    private List<Fragment> lostFragments = new List<Fragment>();
    public bool completed;

    private void Start()
    {
        UpdateNote();
    }

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

    public List<Fragment> GetFragments()
    {
      return fragments;
    }

    public Fragment GetFragment(int index)
    {
      return fragments[index];
    }

    public Fragment GetRandomLostFragment()
    {
      return lostFragments[Random.Range(0, lostFragments.Count)];
    }

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

    public List<Fragment> GetLostFragments()
    {
        return lostFragments;
    }
}
