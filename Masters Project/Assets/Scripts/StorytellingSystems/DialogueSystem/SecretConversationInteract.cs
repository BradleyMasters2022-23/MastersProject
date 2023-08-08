/* ================================================================================================
 * Author - Ben Schuster   
 * Date Created - June 12th, 2023
 * Last Edited - June 12th, 2023 by Ben Schuster
 * Description - Override for conversation interat for the secret terminals. Manages
 * getting the new screen and destroying it once a conversation has been read
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretConversationInteract : ConversationInteract
{
    [Header("Secrets")]

    [Tooltip("How much stronger than the current depth should the crystal spawn at")]
    [SerializeField] private int crystalBuff;
    [SerializeField] private CrystalInteract crystal;
    [SerializeField] private MoveTo orbRef;

    [SerializeField] private Animator anim;
    [SerializeField] private RandomCallUI newCallUI;

    private bool available = true;

    private void Awake()
    {
        newCallUI.InitRandomCall();
    }

    /// <summary>
    /// On interact, open the menu
    /// </summary>
    public override void OnInteract()
    {
        if (!available) return;

        if(GameManager.instance.CurrentState != GameManager.States.MAINMENU)
            GameManager.instance.ChangeState(GameManager.States.GAMEMENU);
        newCallUI.gameObject.SetActive(true);

        //StartCoroutine(CheckForComplete());
    }
    /// <summary>
    /// This can only be interacted with if a call is available
    /// </summary>
    /// <returns></returns>
    public override bool CanInteract()
    {
        return available;
    }

    /// <summary>
    /// Mark conversation as complete. Passed into the display dialogue via RandomCallUI
    /// </summary>
    public void OnCallComplete()
    {
        available = false;
        //SpawnNewCrystal();
        orbRef.Move();
        SetOffline();
    }

    /// <summary>
    /// Spawn a new crystal at a stronger level than normal
    /// </summary>
    public void SpawnNewCrystal()
    {
        crystal.RandomizeCrystalSetStats(MapLoader.instance.PortalDepth() + crystalBuff);
        crystal.gameObject.SetActive(true);
    }
    /// <summary>
    /// Set the terminal to offline, handling visuals and functionality
    /// </summary>
    private void SetOffline()
    {
        // turn off the interact
        ConvoRefManager.instance.CloseAllScreens();
        gameObject.SetActive(false);

        // set the animator to play through the sequence of offline mode
        anim?.SetTrigger("play");
    }
}
