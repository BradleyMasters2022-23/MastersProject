/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - November 28, 2022
 * Last Edited - November 28, 2022 by Ben Schuster
 * Description - Base class for all UI menus to help with UI flow
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Masters.UI;
using System.Linq;

public abstract class UIMenu : MonoBehaviour
{
    [Tooltip("On controller, which option is selected when screen is opened?" +
        "Top items will take priority but only activate if they're ENABLED on open")]
    [SerializeField] private GameObject[] controllerDefaultPriority;

    [Tooltip("Sound when Menu is opened")]
    [SerializeField] private AudioClipSO openMenu;

    [Tooltip("Whether this can be easily closed with the back system." +
        "Use on one way screens, such as the death screen")]
    [SerializeField] private bool closable = true;

    /// <summary>
    /// Whether this screen can close
    /// </summary>
    public bool Closable { get { return closable; } }

    private AudioSource source;

    /// <summary>
    /// What was the last gameobject selected, if any?
    /// </summary>
    private Selectable lastSelected;

    /// <summary>
    /// Animator for this UI menu as a whole.
    /// Can be used to animate the open and close events
    /// </summary>
    protected Animator animator;

    /// <summary>
    /// Try get animator reference
    /// </summary>
    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        source = gameObject.AddComponent<AudioSource>();
    }

    /// <summary>
    /// When the menu is opened, add itself to menu stack. Trigger any animation
    /// </summary>
    protected virtual void OnEnable()
    {
        // If the instance has not been initialized yet, wait
        if(GameManager.instance == null)
        {
            StartCoroutine(WaitOpen());
        }
        else
        {
            GameManager.instance.PushMenu(this);
            OpenMenu();
        }
    }

    private IEnumerator WaitOpen()
    {
        while (GameManager.instance == null)
            yield return null;

        GameManager.instance.PushMenu(this);
        OpenMenu();
    }

    /// <summary>
    /// Open the menu, call animator if possible
    /// </summary>
    public virtual void OpenMenu()
    {
        openMenu.PlayClip(source);

        if (animator != null && animator.HasStateStr("Open"))
        {
            animator.StartPlayback();
            animator.SetBool("Open", true);
        }
    }

    /// <summary>
    /// When inputted by an on-screen button, tell manager to close
    /// </summary>
    public virtual void CloseButton()
    {
        //Debug.Log("Close button called");
        GameManager.instance.CloseTopMenu();
        lastSelected = null;
    }

    /// <summary>
    /// Close the menu and animator, if able
    /// </summary>
    public virtual void Close()
    {
        // If there is an animator, play animation before closing
        if (animator != null)
        {
            // If the animator has an open/close state, use that instead
            if(animator.HasStateStr("Open"))
            {
                animator.SetBool("Open", false);
            }
            else // Otherwise, close it and stop its playback now
            {
                animator.StopPlayback();
                CloseFunctionality();
            }
            
            lastSelected = null;
        }
        // Otherwise, close by unloading
        else
        {
            CloseFunctionality();
        }
    }

    /// <summary>
    /// Move the pointer to the appropriate UI option
    /// </summary>
    public void TopStackFunction()
    {
        // re-enable any interactable options 
        if(groupManager != null)
        {
            groupManager.interactable = true;
        }
        
        // if last object was set and enabled, go to it
        if (lastSelected != null && lastSelected.isActiveAndEnabled && lastSelected.IsInteractable())
        {
            EventSystem.current.SetSelectedGameObject(lastSelected.gameObject);
        }
        else // otherwise, choose the first available one
        {
            Selectable[] allOptions = GetComponentsInChildren<Selectable>();
            foreach (Selectable option in allOptions)
            {
                if(option.IsInteractable())
                {
                    EventSystem.current.SetSelectedGameObject(option.gameObject);
                    break;
                }
            }

            if(EventSystem.current.currentSelectedGameObject == null)
            {
                Debug.LogError($"[UI] On menu named {name}, there is no gameobject available for the UI to select!");
            }
        }

        /*   Old Settings. Was too jank and too much work to keep up with default options
        // If type is mouse, dont auto assign 
        if (InputManager.CurrControlScheme == InputManager.ControlScheme.KEYBOARD)
        {
            GameManager.instance.ClearPointer();
            //Debug.Log("Clearing pointer for M&K");
            return;
        }

        // If nothing, dont do anything
        if (lastSelected == null && controllerDefaultPriority == null)
        {
            // if nothing is last selected and no controller priority, get the first interactable
            Selectable[] s = GetComponentsInChildren<Selectable>(false);
            foreach (var o in s)
            {
                if (o.gameObject.activeInHierarchy && o.IsInteractable())
                {
                    EventSystem.current.SetSelectedGameObject(o.gameObject);
                    break;
                }
            }


            return;
        }
        // If there is a last selectd option, set selection to that
        else if (lastSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(lastSelected);
        }
        // Otherwise, use the default
        else
        {
            // find the first valid item to use. Valid item is any item that is active and interactable
            foreach (var o in controllerDefaultPriority)
            {
                if (o.activeInHierarchy && o.GetComponent<Selectable>() != null && o.GetComponent<Selectable>().IsInteractable())
                {
                    EventSystem.current.SetSelectedGameObject(o);
                    break;

                }
            }
        }

        Debug.Log($"Set the event select to {EventSystem.current.currentSelectedGameObject.name}");
        */
    }


    /// <summary>
    /// Save the currently selected button to this internal manager
    /// </summary>
    public void StackSave()
    {
        if(EventSystem.current.currentSelectedGameObject != null)
        {
            lastSelected = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
        }
    }

    /// <summary>
    /// Call the specific close functionality needed. 
    /// </summary>
    public abstract void CloseFunctionality();

    /// <summary>
    /// canvas group that enables and disables all interactions 
    /// </summary>
    CanvasGroup groupManager;
    /// <summary>
    /// set the menu to background, preventing it from being selectable via UI navigation
    /// </summary>
    public virtual void SetBackground()
    {
        if(groupManager == null)
            groupManager = GetComponent<CanvasGroup>();

        //groupManager.interactable = false;
    }

    /// <summary>
    /// Call game manager to close every screen up until this menu
    /// </summary>
    public void CloseToThisScreen()
    {
        GameManager.instance.CloseToMenu(this);
    }
}
