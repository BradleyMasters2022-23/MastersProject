/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - November 28, 2022
 * Last Edited - November 28, 2022 by Ben Schuster
 * Description - Manages closing game object based menus
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;

public class GameObjectMenu : UIMenu
{
    [SerializeField] UnityEvent onCloseActions;

    /// <summary>
    /// Close the current game object. Public so the animator can call it
    /// </summary>
    public override void CloseFunctionality()
    {
        gameObject.SetActive(false);
    }

    public override void Close()
    {
        onCloseActions.Invoke();
        base.Close();
    }
}
