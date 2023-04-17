/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - December 2, 2022
 * Last Edited - April 15, 2022 by Ben Schuster
 * Description - Functionality for the confirmation box system
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class ConfirmationBox : MonoBehaviour
{
    /// <summary>
    /// The action to take when the confirmation window is clicked
    /// </summary>
    private System.Action onConfirm;

    [SerializeField] private string defaultText;
    [SerializeField] private TextMeshProUGUI bonusText;

    /// <summary>
    /// Request a confirmation, passing in the requested action
    /// </summary>
    /// <param name="action">action requesting to happen</param>
    public void RequestConfirmation(System.Action action, string bonusText = "")
    {
        if(bonusText != "")
            this.bonusText.text = bonusText;
        else
            this.bonusText.text = defaultText;

        onConfirm = action;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Execute this when button is confirmed and close the box
    /// </summary>
    public void ConfirmAction()
    {
        onConfirm?.Invoke();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// When this is disabled, clear the action
    /// </summary>
    private void OnDisable()
    {
        bonusText.text = defaultText;
        onConfirm = null;
    }
}
