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
using Sirenix.OdinInspector;

public class FragmentInteract : MonoBehaviour, Interactable
{
    [Tooltip("A note to pull from. Will always pull from this override.")]
    [SerializeField] private NoteObject noteOverride;
    [Tooltip("A fragment to use. Takes priority over note override.")]
    [SerializeField] private Fragment fragmentOverride;
    [SerializeField, ReadOnly] private Fragment fragment;
    [SerializeField, ReadOnly] private NoteObject note;

    [SerializeField, ReadOnly] private NoteFoundUI ui;

    private void Awake()
    {
        // Debug.Log("Trying to awake fragment interact");

        if(fragmentOverride != null || noteOverride != null)
        {
            PrepareNote();
        }
    }

    /// <summary>
    /// initializes fragment
    /// </summary>
    public void SetUp(Fragment frag)
    {
        fragment = frag;
    }

    public void PrepareNote()
    {
        // if the fragment is already assigned, then it was already prepared
        if (fragment != null)
            return;

        // if no note manager, just stop
        if (AllNotesManager.instance == null)
        {
            DestroyFrag();
            return;
        }

        // If fragment override assigned, apply them both
        if(fragmentOverride != null)
        {
            note = noteOverride;
            fragment = fragmentOverride;

        }
        // If no fragment assigned but note assigned, get a random fragment from that note
        else if(noteOverride != null)
        {
            Debug.Log("Checking logic for note override");
            note = noteOverride;

            // If the override note is already complete, destroy fragment
            // can be changed later to give repeats?
            if (AllNotesManager.instance.CheckNoteComplete(noteOverride))
            {
                Debug.Log("Note set to complete, trying to get fragment");
                DestroyFrag();
                return;
            }
            else
            {
                Debug.Log("Trying to get lost fragment from overriden note");
                fragment = noteOverride.GetRandomLostFragment();
            }
        }
        // Otherwise, pull fronm pool
        else
        {
            note = AllNotesManager.instance.GetRandomLostNote();
            if (note != null)
            {
                fragment = note.GetRandomLostFragment();
            }
        }
            

        // If it failed to get a new one, destroy it
        if (fragment == null)
        {
            Debug.Log("No fragment found, destroying frag instead");
            DestroyFrag();
        }

        // get the one thats combined with player object ensure we get the instanced one
        ui = PlayerTarget.p.GetComponentInChildren<NoteFoundUI>(true);
    }

    public void OnInteract()
    {
        if (GameManager.instance.CurrentState != GameManager.States.GAMEPLAY && GameManager.instance.CurrentState != GameManager.States.HUB)
        {
            Debug.Log("Not in a state where the player can interact with this object");
            return;
        }

        // make sure theres a fragment to use
        if (fragment == null)
        {
            Destroy(gameObject);
            return;
        }

        ui.LoadFragment(this);
        ui.OpenScreen();

        AllNotesManager.instance.FragmentFound(fragment);
    }

    /// <summary>
    /// Can interact if the assigned interact is not null
    /// </summary>
    /// <returns></returns>
    public bool CanInteract()
    {
        return fragment != null;
    }

    public Fragment GetFragment()
    {
        return fragment;
    }
    public NoteObject GetNote()
    {
        return note;
    }

    public void DestroyFrag()
    {
        Destroy(gameObject);
    }

}
