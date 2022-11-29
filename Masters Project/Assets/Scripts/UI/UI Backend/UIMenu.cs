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

public abstract class UIMenu : MonoBehaviour
{
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
    }

    /// <summary>
    /// When the menu is opened, add itself to menu stack. Trigger any animation
    /// </summary>
    protected virtual void OnEnable()
    {
        GameManager.instance.PushMenu(this);
        OpenMenu();
    }

    /// <summary>
    /// Open the menu, call animator if possible
    /// </summary>
    public virtual void OpenMenu()
    {
        if(animator != null)
        {
            animator.SetBool("open", true);
        }
    }

    /// <summary>
    /// When inputted by an on-screen button, tell manager to close
    /// </summary>
    public virtual void CloseButton()
    {
        GameManager.instance.CloseTopMenu();
    }

    /// <summary>
    /// Close the menu and animator, if able
    /// </summary>
    public void Close()
    {
        // If there is an animator, play animation before closing
        if (animator != null)
        {
            animator.SetBool("open", false);
        }
        // Otherwise, close by unloading
        else
        {
            CloseFunctionality();
        }
    }

    /// <summary>
    /// Call the specific close functionality needed. 
    /// </summary>
    public abstract void CloseFunctionality();
}
