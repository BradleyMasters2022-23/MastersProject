using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;

public class SelectUpgradeUI : MonoBehaviour
{
    GameControls c;
    InputAction esc;

    [SerializeField] private UpgradeSelectModule[] options;

    private UpgradeInteract caller;

    /// <summary>
    /// Initialize inputs [TEMP]
    /// </summary>
    private void Awake()
    {
        c = new GameControls();
        esc = c.PlayerGameplay.Pause;
        esc.performed += CloseScreen;
    }

    /// <summary>
    /// Load in necessary data for the current selection
    /// </summary>
    /// <param name="choices">choices of upgrades to choose from</param>
    /// <param name="c">the caller of this function</param>
    public void LoadOptions(UpgradeObject[] choices, UpgradeInteract c)
    {
        for(int i = 0; i < options.Length; i++)
        {
            options[i].InitializeUIElement(choices[i]);
        }

        caller = c;
    }

    /// <summary>
    /// Open the screen, change state
    /// </summary>
    public void OpenScreen()
    {
        GameManager.instance.ChangeState(GameManager.States.GAMEMENU);
        gameObject.SetActive(true);
        esc.Enable();
    }

    /// <summary>
    /// Close the screen, change state. For input system
    /// </summary>
    /// <param name="c"></param>
    private void CloseScreen(InputAction.CallbackContext c) 
    {
        CloseScreen();
    }
    /// <summary>
    /// Close the screen, change state
    /// </summary>
    public void CloseScreen()
    {
        esc.Disable();
        gameObject.SetActive(false);
        GameManager.instance.ChangeState(GameManager.States.GAMEPLAY);
    }

    /// <summary>
    /// What happens when an upgrade is chosen. Called by unity button.
    /// </summary>
    /// <param name="index"></param>
    public void ChooseUpgrade(int index)
    {
        // TODO - feed this upgrade to the PUM
        UpgradeObject chosenUpgrade = options[index].upgradeData;

        // Send upgrade data to upgrade manager to be applied
        if(PlayerUpgradeManager.instance != null)
            PlayerUpgradeManager.instance.AddUpgrade(chosenUpgrade);

        // Tell container that its request has been granted and it can continue the game
        caller.UpgradeSelected();

        Debug.Log(chosenUpgrade.displayName + " was selected!");
    }

    /// <summary>
    /// What happens when enhancing an upgrade is chosen. Caled by unity button.
    /// </summary>
    public void EnhanceUpgrade()
    {
        // TODO - Implement this feature later
    }

    /// <summary>
    /// Reset the screen to its original initialization
    /// </summary>
    public void ResetScreen()
    {
        foreach (UpgradeSelectModule m in options)
            m.ClearUI();

        CloseScreen();

        caller = null;
    }
}
