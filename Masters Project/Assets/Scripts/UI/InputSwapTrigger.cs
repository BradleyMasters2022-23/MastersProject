/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - July 6th, 2023
 * Last Edited - July 6th, 2023 by Ben Schuster
 * Description - Perform functionality depending on which controller scheme is applied
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputSwapTrigger : MonoBehaviour
{
    [Tooltip("Channel thats called when input controller swaps")]
    [SerializeField] private ChannelControlScheme onSchemeSwap;
    [Tooltip("Events that execute when switching to keyboard and mouse")]
    [SerializeField] private UnityEvent onKeyboardScheme;
    [Tooltip("Events that execute when switching to a gamepad")]
    [SerializeField] private UnityEvent onControllerScheme;
    private void OnEnable()
    {
        onSchemeSwap.OnEventRaised += SwapControls;
        SwapControls(InputManager.CurrControlScheme);
    }
    private void OnDisable()
    {
        onSchemeSwap.OnEventRaised -= SwapControls;
    }

    /// <summary>
    /// Invoke relevant events on controller input swap
    /// </summary>
    /// <param name="controller">Input type swap</param>
    private void SwapControls(InputManager.ControlScheme controller)
    {
        switch(controller)
        {
            case InputManager.ControlScheme.KEYBOARD:
                {
                    onKeyboardScheme.Invoke();
                    break;
                }
            case InputManager.ControlScheme.CONTROLLER:
                {
                    onControllerScheme.Invoke();
                    break;
                }
        }
    }
}
