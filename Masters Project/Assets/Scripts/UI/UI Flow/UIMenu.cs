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
    /// canvas group that enables and disables all interactions 
    /// </summary>
    CanvasGroup groupManager;

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

        if (animator != null)
        {
            animator.enabled = true;
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
        // old function.
        return;
    }


    /// <summary>
    /// Save the currently selected button to this internal manager
    /// </summary>
    public void StackSave()
    {
        //if(EventSystem.current.currentSelectedGameObject != null)
        //{
        //    lastSelected = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
        //}
    }

    /// <summary>
    /// Call the specific close functionality needed. 
    /// </summary>
    public abstract void CloseFunctionality();

    
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
