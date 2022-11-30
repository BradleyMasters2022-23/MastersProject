/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - November 28, 2022
 * Last Edited - November 28, 2022 by Ben Schuster
 * Description - Manages closing additive based menus
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AddativeMenu : UIMenu
{
    /// <summary>
    /// Unload the current scene. Public so the animator can call it
    /// </summary>
    public override void CloseFunctionality()
    {
        int currScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.UnloadSceneAsync(currScene);
    }
}
