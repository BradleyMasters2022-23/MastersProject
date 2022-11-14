using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
    public Transform point;
    public float interactDistance;
    private PlayerController player;
    private GameControls controller;
    private InputAction interact;
    private bool interacting = false;

    private void Awake() {
        controller = new GameControls();
        interact = controller.PlayerGameplay.Interact;
        interact.Enable();
        interact.performed += Interact;
    }

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }

    private void Interact(InputAction.CallbackContext context)
    {
        RaycastHit hit;
        interacting = Physics.Raycast(point.position, point.forward, out hit, interactDistance);

        if(interacting)
        {
            if(hit.transform.GetComponent<Interactable>() != null)
            {
                hit.transform.GetComponent<Interactable>().OnInteract(player);
            }
            else
            {
                Debug.Log("nothin to interact with");
            }
        }
    }
}
