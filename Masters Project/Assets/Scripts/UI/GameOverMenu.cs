/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 28th, 2022
 * Last Edited - October 28th, 2022 by Ben Schuster
 * Description - Button functionality for game over menu
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    [Tooltip("The game over screen game object")]
    [SerializeField] private GameObject gameOverScreen;
    [Tooltip("Channel that triggers game manager state changes")]
    [SerializeField] private ChannelGMStates onStateChangeChannel;


    private void Start()
    {
        // Make sure death screen is disabled
        gameOverScreen.SetActive(false);
    }

    private void OnEnable()
    {
        // Make sure load death screen is bound to channel
        onStateChangeChannel.OnEventRaised += LoadDeathScreen;
    }
    private void OnDisable()
    {
        // Make sure to unload function to prevent crashing
        onStateChangeChannel.OnEventRaised -= LoadDeathScreen;
    }

    private void LoadDeathScreen(GameManager.States _newState)
    {
        // if going to game over state, enable game over screen
        if(_newState == GameManager.States.GAMEOVER)
        {
            gameOverScreen.SetActive(true);
        }
    }

    /// <summary>
    /// Request the ability to quit the game
    /// </summary>
    public void RequestQuitGame()
    {
        FindObjectOfType<ConfirmationBox>(true).RequestConfirmation(QuitGame);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    
    public void LoadToHubOldSystem()
    {
        RoomGenerator.instance.ReturnToHub();
    }

    /// <summary>
    /// Request the ability to return to hub
    /// </summary>
    public void RequestHUBReturn()
    {
        FindObjectOfType<ConfirmationBox>(true).RequestConfirmation(LoadToHubNewSystem);
    }

    public void LoadToHubNewSystem()
    {
        MapLoader.instance.ReturnToHub();
    }

    public void ReturnToMainMenu()
    {
        GameManager.instance.ChangeState(GameManager.States.MAINMENU);
    }

}
