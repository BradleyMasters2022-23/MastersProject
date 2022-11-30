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
    [SerializeField] private Fragment fragment;
    private NoteFoundUI ui;
    private bool dataSent = false;

    private void Awake()
    {
        ui = FindObjectOfType<NoteFoundUI>(true);
    }

    /// <summary>
    /// initializes fragment
    /// </summary>
    public void SetUp(Fragment frag)
    {
        fragment = frag;
    }

    public override void OnInteract(PlayerController player)
    {
        if (ui == null) {
            Debug.Log("No UI found.");
        }

        if (GameManager.instance.CurrentState != GameManager.States.GAMEPLAY && GameManager.instance.CurrentState != GameManager.States.HUB)
        {
            Debug.Log("Not in a state where the player can interact with this object");
            return;
        }

        if (!dataSent)
        {
            ui.LoadFragment(this);

            dataSent = true;
        }

        ui.OpenScreen();
        PlayerNotesManager.instance.FindFragment(fragment);

    }

    public Fragment GetFragment()
    {
        return fragment;
    }

}
