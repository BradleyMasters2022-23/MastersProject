using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RebindTester : MonoBehaviour
{
    private InputAction p;

    private void Start()
    {
        p = InputManager.Controls.PlayerGameplay.Jump;
        p.performed += Pew;
        p.Enable();
        InputManager.SwapActionMap(InputManager.Controls.PlayerGameplay);
    }

    void Pew(InputAction.CallbackContext ctx = default)
    {
        Debug.Log("Pew");
    }
}
