/* ================================================================================================
 * Author - Ben Schuster
 * Date Created - June 26th, 2023
 * Last Edited - June 26th, 2023 by Ben 
 * Description - Clears player input on game end. Prevents an error related to a bug in unity's input system
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ErrorClearer : MonoBehaviour
{
    PlayerInput p;
    void Awake()
    {
        p = GetComponent<PlayerInput>();
    }

    private void OnDisable()
    {
        if (p != null)
        {
            p.actions = null;
            p.enabled = false;
        }
    }
}
