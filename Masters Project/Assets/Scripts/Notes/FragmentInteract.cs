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
    /// called exactly once, initializes container
    /// </summary>
    public void Awake()
    {
        fragment = AllNotesManager.instance.GetRandomLostNote().GetRandomLostFragment();
    }

    public override void OnInteract(PlayerController player)
    {
        PlayerNotesManager.instance.FindFragment(fragment);
        // somehow bring up a menu here?
        Debug.Log("It's a note.");
        Destroy(this);
    }
}
