using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SchemeNameTest : MonoBehaviour
{
    SchemeControls controls;


    private void Start()
    {
        controls = new SchemeControls();
        controls.Enable();
        controls.ObserveGamePad.SwapControls.performed += PrintOutput;
        controls.ObserveMK.SwapControls.performed += PrintOutput;
    }

    private void PrintOutput(InputAction.CallbackContext c)
    {
        Debug.Log(c.control.name);
    }
}
