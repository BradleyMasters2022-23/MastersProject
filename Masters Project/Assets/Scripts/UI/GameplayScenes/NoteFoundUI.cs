using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;

public class NoteFoundUI : MonoBehaviour
{
    GameControls c;
    InputAction esc;
    private EventSystem eventSystem;
    private Fragment displayFrag;
    private FragmentInteract caller;

    [Tooltip("Reference to the title textbox")]
    [SerializeField] private TextMeshProUGUI title;
    [Tooltip("Reference to the description textbox")]
    [SerializeField] private TextMeshProUGUI content;

    private void Awake()
    {
        c = new GameControls();
        esc = c.PlayerGameplay.Pause;
        esc.performed += CloseScreen;
    }

    public void LoadFragment(FragmentInteract f)
    {
        displayFrag = f.GetFragment();
        caller = f;

        this.title.text = displayFrag.GetNoteName();
        this.content.text = displayFrag.content;
    }

    public void OpenScreen()
    {
        if (eventSystem == null)
        {
            eventSystem = EventSystem.current;
        }

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
        if(caller != null)
        {
            caller.DestroyFrag();
        }
        caller = null;
        esc.Disable();
        gameObject.SetActive(false);
    }

}
