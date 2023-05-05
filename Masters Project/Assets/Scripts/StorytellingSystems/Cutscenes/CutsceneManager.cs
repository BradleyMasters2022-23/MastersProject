using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.InputSystem;
using TMPro;
using Sirenix.OdinInspector;


public class CutsceneManager : MonoBehaviour
{
    [Header("Main Systems")]
    [Tooltip("Reference to the video player")]
    [SerializeField] private VideoPlayer videoPlayer;
    [Tooltip("Render image to use")]
    [SerializeField] private RawImage videoRenderImg;
    [Tooltip("The loaded video to play")]
    [SerializeField, ReadOnly] private VideoClip loadedCutscene;
    /// <summary>
    /// Whether or not this cutscene is allowed to play
    /// Connect to the save system
    /// </summary>
    private bool allowedToPlay = true;

    [Header("Transitions")]
    [Tooltip("Image used as the background of the video")]
    [SerializeField] private Image background;
    [Tooltip("Time it takes to fade into the video")]
    [SerializeField] private float fadeInTime;
    [Tooltip("Time it takes to fade out of the video")]
    [SerializeField] private float fadeOutTime;
    [Tooltip("Time it takes to fade out of the video if the video was skipped")]
    [SerializeField] private float skippedFadeOutTime;

    [Header("Pausing and Skipping")]
    [Tooltip("Ref to the pause screen object")]
    [SerializeField] private GameObject pauseScreen;
    [Tooltip("Ref to the setting screen object")]
    [SerializeField] private GameObject settingScreen;
    [Tooltip("Ref to the pause prompt box")]
    [SerializeField] private GameObject pausePrompt;
    [Tooltip("Ref to the prompt saying to skip")]
    [SerializeField] private TextMeshProUGUI promptText;
    [Tooltip("Time the note remains on screen with no input")]
    [SerializeField] private float noteDisplayTime;
    /// <summary>
    /// Routine tracking the fade in and our of the prompt
    /// </summary>
    private Coroutine promptFadeRoutine;
    /// <summary>
    /// Routine tracking the prompt life
    /// </summary>
    private Coroutine promptRoutine;
    /// <summary>
    /// Scaled timer tracking the prompt life
    /// </summary>
    private ScaledTimer promptLifeTracker;

    /// <summary>
    /// Reference to the game controls 
    /// </summary>
    private GameControls playerControls;
    private InputAction pause;
    private InputAction showHide;

    #region Main Player

    private IEnumerator Start()
    {
        // wait until the controls are initialized
        yield return new WaitUntil(() => GameManager.controls != null);
        playerControls = GameManager.controls;

        // setup controls 
        pause = playerControls.Cutscene.Pause;
        pause.performed += TogglePause;
        showHide = playerControls.Cutscene.Any;
        showHide.performed += ShowPrompt;

        // prepare life tracker
        promptLifeTracker = new ScaledTimer(noteDisplayTime, false);

        // assign audio system
        videoPlayer.SetTargetAudioSource(0, GetComponent<AudioSource>());
    }

    public void PrepareCutscene(VideoClip newCutscene)
    {
        loadedCutscene = newCutscene;
        videoPlayer.clip = newCutscene;
        videoPlayer.Prepare();
    }

    public void BeginCutscene()
    {
        if(allowedToPlay && loadedCutscene!=null)
            StartCoroutine(PlayCutscene());
    }

    /// <summary>
    /// Handle the logic for actually playing the cutscene
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayCutscene()
    {
        // Disable pausing just to be safe in big prevention
        playerControls.PlayerGameplay.Pause.Disable();

        // turn on the screen
        yield return StartCoroutine(LoadScreen(true, fadeInTime));

        // Play the video
        videoRenderImg.enabled = true;
        videoPlayer.Play();

        playerControls.UI.Disable();
        playerControls.PlayerGameplay.Disable();
        playerControls.Cutscene.Enable();

        // Track time only if its playing. Dont if its paused
        float timeElapsed = 0;
        while (timeElapsed < videoPlayer.length)
        {
            if (!videoPlayer.isPaused)
                timeElapsed += Time.deltaTime;

            yield return null;
        }
        // Disable cutscene controls and reenable gameplay. 
        // Keep pause disabled to prevent any funky business
        playerControls.Cutscene.Disable();
        playerControls.PlayerGameplay.Enable();
        playerControls.PlayerGameplay.Pause.Disable();

        // Disable on cutscene end stuff
        if (promptRoutine != null)
            StopCoroutine(promptRoutine);
        if(promptFadeRoutine!=null)
            StopCoroutine(promptFadeRoutine);

        promptText.enabled = false;
        videoRenderImg.enabled = false;

        StartCoroutine(FadePrompt(false, 0.3f));
        // By now, video has stopped, close screen
        yield return StartCoroutine(LoadScreen(false, fadeOutTime));

        // Disable pausing just to be safe in big prevention
        playerControls.PlayerGameplay.Pause.Enable();
    }

    /// <summary>
    /// Fade in and out the screen
    /// </summary>
    /// <param name="active">Whether to activate the fade effect</param>
    /// <returns></returns>
    private IEnumerator LoadScreen(bool active, float dur)
    {
        // Get original and target color
        Color originalCol = background.color;
        Color targetCol = originalCol;
        if (active)
        {
            targetCol.a = 1;
        }
        else
        {
            targetCol.a = 0;
        }

        // Lerp through with the given timer
        ScaledTimer lerpTimer = new ScaledTimer(dur, false);
        while (!lerpTimer.TimerDone())
        {
            background.color = Color.Lerp(originalCol, targetCol, lerpTimer.TimerProgress());
            yield return null;
        }
        // make sure the target color is reached
        background.color = targetCol;

        yield return null;
    }

    #endregion

    #region Pausing and Skipping

    public void SkipCutscene()
    {
        StopAllCoroutines();
        playerControls.Cutscene.Disable();
        pauseScreen.SetActive(false);
        pausePrompt.SetActive(false);
        videoRenderImg.enabled = false;
        Cursor.lockState= CursorLockMode.Locked;

        videoPlayer.Stop();
        StartCoroutine(LoadScreen(false, skippedFadeOutTime));

        
        playerControls.PlayerGameplay.Enable();
        playerControls.PlayerGameplay.Pause.Enable();
    }

    /// <summary>
    /// Toggle the pause of the cutscene
    /// </summary>
    /// <param name="c">input context</param>
    private void TogglePause(InputAction.CallbackContext c)
    {
        if(pauseScreen.activeSelf)
        {
            if (settingScreen.activeSelf)
                settingScreen.SetActive(false);
            else
                ResumeCutscene();
        }
        else
            PauseCutscene();
    }

    /// <summary>
    /// Pause the cutscene
    /// </summary>
    public void PauseCutscene()
    {
        Cursor.lockState = CursorLockMode.Confined;
        settingScreen.SetActive(false);
        pauseScreen.SetActive(true);
        videoPlayer.Pause();
        promptText.enabled = false;

        if(promptRoutine!= null)
        {
            StopCoroutine(promptRoutine);
            promptRoutine= null;
        }
            
    }

    /// <summary>
    /// Resume the cutscene
    /// </summary>
    public void ResumeCutscene()
    {
        Cursor.lockState = CursorLockMode.Locked;
        pauseScreen.SetActive(false);
        videoPlayer.Play();
    }

    /// <summary>
    /// Show the prompt notifying player that the cutscene can be paused
    /// </summary>
    private void ShowPrompt(InputAction.CallbackContext c)
    {
        // dont show if the cutscene is paused or not playing at all
        if (!videoPlayer.isPlaying)
            return;

        if(!promptText.enabled)
            promptFadeRoutine = StartCoroutine(FadePrompt(true, 1f));

        promptLifeTracker.ResetTimer();
        if (promptRoutine == null)
            promptRoutine = StartCoroutine(CheckPrompt());
            
    }
    /// <summary>
    /// Hide the prompt notifying the cutscene can be paused
    /// </summary>
    private void HidePrompt()
    {
        StartCoroutine(FadePrompt(false, 1f));
    }

    /// <summary>
    /// Fade the prompt in and out over time
    /// </summary>
    /// <param name="active"></param>
    /// <param name="dur"></param>
    /// <returns></returns>
    private IEnumerator FadePrompt(bool active, float dur)
    {
        // make visible to start
        promptText.enabled = true;

        // determine target alpha color
        Color originalCol = promptText.color;
        Color targetCol = originalCol;
        if (active)
        {
            targetCol.a = 1;
        }
        else
        {
            targetCol.a = 0;
        }

        // Lerp through with the given timer
        ScaledTimer lerpTimer = new ScaledTimer(dur, false);
        while (!lerpTimer.TimerDone())
        {
            promptText.color = Color.Lerp(originalCol, targetCol, lerpTimer.TimerProgress());
            yield return null;
        }
        // make sure the target color is reached
        promptText.color = targetCol;
        promptText.enabled = active;
    }

    /// <summary>
    /// Continually check if the prompt should vanish or not
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckPrompt()
    {
        // wait for the timer to compelete
        while(!promptLifeTracker.TimerDone())
        {
            yield return new WaitForSecondsRealtime(0.5f);
        }

        // If timer completed and its still active, hide the prompt
        if(promptText.isActiveAndEnabled)
            HidePrompt();
    }

    #endregion
}
