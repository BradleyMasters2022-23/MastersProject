/* ================================================================================================
 * Author - Soma Hannon
 * Date Created - October 25, 2022
 * Last Edited - November 11, 2022 by Soma Hannon
 * Description - Defines a spawnable container of a single fragment.
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentInteract : Interactable
{
    [Tooltip("The fragment this container represents.")]
    [SerializeField] private Fragment fragment;

    /// <summary>
    /// ensures that fragment is not null and calls SetUp
    /// </summary>
    private void Start()
    {
        if (fragment is null)
        {
            Destroy(this);
        }

        if(fragment != null)
        {
            SetUp(fragment);
        }
    }

    /// <summary>
    /// called exactly once, initializes container
    /// </summary>
    public void SetUp(Fragment obj)
    {
        fragment = obj;
    }

    public override void OnInteract(PlayerController player)
    {
        PlayerNotesManager.instance.FindFragment(fragment);
        // somehow bring up a menu here?
        Debug.Log("It's a note.");
        Destroy(this);
    }
}
