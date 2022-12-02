/* ================================================================================================
 * Author - Soma Hannon (base code - Ben Schuster)
 * Date Created - October 25, 2022
 * Last Edited - November 17, 2022 by Ben Schuster
 * Description - Physical object that triggers the upgrade select screen.
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradeInteract : Interactable 
{
    [Tooltip("How many options do we want to give the player?")]
    [SerializeField] private int numOfChoices = 3;

    /// <summary>
    /// The UI stuff to send data to
    /// </summary>
    private SelectUpgradeUI ui;

    private bool dataSent = false;

    private void Awake()
    {
        ui = FindObjectOfType<SelectUpgradeUI>(true);
    }

    public override void OnInteract(PlayerController player) 
    {
        // Make sure player is in gameplay or hub state to prevent multi clicking
        if(GameManager.instance.CurrentState != GameManager.States.GAMEPLAY && GameManager.instance.CurrentState != GameManager.States.HUB)
        {
            Debug.Log("Not in a state where the player can interact with ths object");
            return;
        }

        if(!dataSent)
        {
            // Get upgrade options
            UpgradeObject[] options = AllUpgradeManager.instance.GetRandomOptions(numOfChoices);

            // Load in the new UI options, open the screen
            ui.LoadOptions(options, this);

            dataSent = true;
        }

        ui.OpenScreen();

        //Debug.Log("It's an upgrade!");
    }

    public void UpgradeSelected()
    {
        GetComponentInParent<HallwayLoader>().UpgradeSelected();

        Destroy(gameObject);
    }
}
