/* ================================================================================================
 * Author - Soma Hannon   
 * Date Created - October, 2022
 * Last Edited - June 8th, 2023 by Ben Schuster
 * Description - Handles player interaction input
 * ================================================================================================
 */
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class InteractionManager : MonoBehaviour
{
    private GameControls controller;
    private InputAction interact;

    [Tooltip("How close to be able to interact with something")]
    [SerializeField] private float interactDistance;

    [Tooltip("Tolerance for accuracy to interact. This represents a radius.")]
    [SerializeField] private float interactTolerance;

    [Tooltip("Physics layers to use for interact")]
    [SerializeField] private LayerMask detectableLayer;

    /// <summary>
    /// the current interact being looked at. Null if nothing.
    /// </summary>
    private Interactable currentInteract;

    private bool lastFrameInteractable;

    [Header("Flow Channels")]
    [SerializeField] private ChannelBool onInteractChange;

    #region Initialization

    private void Awake() {
        lastFrameInteractable = false;
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

    private void OnDisable()
    {
        if (controller != null)
        {
            interact.performed -= Interact;
        }
    }

    #endregion

    private void Update()
    {
        // Continually check if the player is currently looking at an interact
        CheckInteract();
    }

    /// <summary>
    /// Check if theres something to interact with
    /// </summary>
    private void CheckInteract()
    {
        RaycastHit hit;

        Debug.Log("checking interact");

        if (Physics.SphereCast(Camera.main.transform.position, interactTolerance, Camera.main.transform.forward, out hit, interactDistance, detectableLayer))
        {
            currentInteract = hit.transform.GetComponent<Interactable>();
            Debug.Log("hit something, trying to get interactable");
        }
        else // If nothing hit, then no interact
        {
            Debug.Log("Cant find interact");
            currentInteract = null;
        }

        // If a new interactable is found this frame, raise the on found events
        if (!lastFrameInteractable && currentInteract != null && currentInteract.CanInteract())
        {
            lastFrameInteractable = true;
            onInteractChange?.RaiseEvent(true);
        }
        // otherwise if the interact was lost OR cannot be interacted, raise the on lost events 
        else if (lastFrameInteractable && (currentInteract == null || !currentInteract.CanInteract()))
        {
            lastFrameInteractable = false;
            onInteractChange?.RaiseEvent(false);
        }
    }

    /// <summary>
    /// Perform the interaction with the target interact
    /// </summary>
    /// <param name="context">input context</param>
    private void Interact(InputAction.CallbackContext context)
    {
        if(currentInteract != null && context.performed)
        {
            currentInteract.OnInteract();

            // Clear any prompt on successful interact
            if (TooltipManager.instance != null)
                TooltipManager.instance.UnloadTooltip();
        } 
    }
}
