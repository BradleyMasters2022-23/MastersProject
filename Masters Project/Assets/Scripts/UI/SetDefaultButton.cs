/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 31th, 2022
 * Last Edited - October 31th, 2022 by Ben Schuster
 * Description - Try to set this the active default botton for controller
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SetDefaultButton : MonoBehaviour
{
    EventSystem system;

    private void OnEnable()
    {
        system = FindObjectOfType<EventSystem>();

        if(system != null)
        {
            system.SetSelectedGameObject(this.gameObject);
        }
    }
}
