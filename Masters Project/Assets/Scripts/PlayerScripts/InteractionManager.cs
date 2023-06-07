using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
    public float interactDistance;
    private PlayerController player;
    private GameControls controller;
    private InputAction interact;
    private bool interacting = false;

    [SerializeField] private LayerMask detectableLayer;

    private void Awake() {
        StartCoroutine(InitializeControls());
    }

    private IEnumerator InitializeControls()
    {
        yield return new WaitUntil(() => GameManager.controls != null);

        controller = GameManager.controls;
        interact = controller.PlayerGameplay.Interact;
        interact.performed += Interact;
        interact.Enable();
    }

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }

    private void Interact(InputAction.CallbackContext context)
    {
        RaycastHit hit;
        interacting = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, interactDistance, detectableLayer);

        //Debug.DrawLine(Camera.main.transform.position, Camera.main.transform.position + Camera.main.transform.forward * interactDistance, Color.red, 1f);

        if(interacting)
        {
            if(hit.transform.GetComponent<Interactable>() != null)
            {
                hit.transform.GetComponent<Interactable>().OnInteract(player);

                // Clear any prompt on successful interact
                if(TooltipManager.instance != null)
                    TooltipManager.instance.UnloadTooltip();
            }
        } 
    }
}
