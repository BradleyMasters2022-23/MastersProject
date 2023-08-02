using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.InputSystem;
using TMPro;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using UnityEngine.Playables;

public class CutsceneManager : MonoBehaviour
{
    /// <summary>
    /// Main cutscene manager
    /// </summary>
    public static CutsceneManager instance;

    [Header("Main Systems")]
    [Tooltip("Reference to the video player")]
    [SerializeField] private VideoPlayer videoPlayer;
    [Tooltip("Render image to use")]
    [SerializeField] private RawImage videoRenderImg;
    [Tooltip("The loaded video to play")]
    [SerializeField, ReadOnly] private VideoClip loadedCutscene;

    /// <summary>
    /// Main speed modifier for playback system
    /// </summary>
    private readonly float playbackSpeed = 2f;

    [Header("Transitions")]
    [Tooltip("Image used as the background of the video")]
    [SerializeField] private Image background;
    [Tooltip("Time it takes to fade into the video")]
    [SerializeField] private float fadeInTime;
    [Tooltip("The initial delay when fade in finishes before starting")]
    [SerializeField] private float startDelay;
    [Tooltip("Time it takes to fade out of the video")]
    [SerializeField] private float fadeOutTime;
    [Tooltip("The initial delay when video finishes and fade out begins")]
    [SerializeField] private float endDelay;

    [Header("Skipping")]
    [Tooltip("Ref to the prompt saying to skip")]
    [SerializeField] private TextMeshProUGUI promptText;

    /// <summary>
    /// Reference to the game controls 
    /// </summary>
    private GameControls playerControls;
    /// <summary>
    /// Input for continuing the cutscene
    /// </summary>
    private InputAction cont;
    /// <summary>
    /// Input for skipping to end of cutscene
    /// </summary>
    private InputAction fastForward;

    /// <summary>
    /// events that execute once the video itself finishes
    /// </summary>
    private UnityEvent onVideoFinishEvents;
    /// <summary>
    /// events that execute once the cutscene is finished and the game fades back into the game
    /// </summary>
    private UnityEvent onCutsceneFadeFinishEvents;

    /// <summary>
    /// Whether the continue was pressed
    /// </summary>
    private bool continuePressed = false;

    #region Main Player

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this);
            return;
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        // assign audio system
        videoPlayer.SetTargetAudioSource(0, GetComponent<AudioSource>());
    }

    private IEnumerator InitializeControls()
    {
        // wait until the controls are initialized
        yield return new WaitUntil(() => GameManager.controls != null);
        playerControls = GameManager.controls;

        // setup controls 
        cont = playerControls.Cutscene.Continue;
        cont.performed += ContinueCutscene;
        fastForward = playerControls.Cutscene.FastForward;
        fastForward.performed += SkipCutscene;

        playerControls.Cutscene.Disable();
    }

    /// <summary>
    /// Initialize controls in enable since manager can be turned off easily
    /// </summary>
    private void OnEnable()
    {
        StartCoroutine(InitializeControls());
        promptText.CrossFadeAlpha(0, 0, true);
    }

    /// <summary>
    /// Remove inputs
    /// </summary>
    private void OnDisable()
    {
        cont.performed -= ContinueCutscene;
        fastForward.performed -= SkipCutscene;

    }

    public void PrepareCutscene(VideoClip newCutscene)
    {
        if (newCutscene == null) return;
        loadedCutscene = newCutscene;
        videoPlayer.clip = newCutscene;
        videoPlayer.Prepare();
    }

    public void BeginCutscene()
    {
        if(loadedCutscene!=null)
            StartCoroutine(PlayCutscene());
    }

    /// <summary>
    /// Load events that execute once the video itself finishes
    /// </summary>
    /// <param name="events"></param>
    public void LoadVideoEndEvents(UnityEvent events)
    {
        onVideoFinishEvents = events;
    }
    /// <summary>
    /// Load events that execute once the cutscene finishes and the fade is gone
    /// </summary>
    /// <param name="events"></param>
    public void LoadCutsceneEndEvents(UnityEvent events)
    {
        onCutsceneFadeFinishEvents= events;
    }

    /// <summary>
    /// Handle the logic for actually playing the cutscene
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayCutscene()
    {
        // Disable pausing just to be safe in big prevention
        playerControls.PlayerGameplay.Pause.Disable();
        playerControls.PlayerGameplay.Interact.Disable();

        // turn on the screen
        
        yield return StartCoroutine(LoadScreen(true, fadeInTime));
        playerControls.Disable();
        playerControls.Cutscene.Enable();
        cont.Disable();

        yield return new WaitForSecondsRealtime(startDelay);

        // Play the video
        videoPlayer.Play();
        yield return new WaitForEndOfFrame();
        videoRenderImg.enabled = true;
        videoPlayer.playbackSpeed = playbackSpeed;
        fastForward.Enable();

        // wait for it to finish playing
        while (videoPlayer.isPlaying)
        {
            yield return null;
        }
        fastForward.Disable();

        // wait for end delay
        yield return new WaitForSecondsRealtime(endDelay);

        // Request a continue input before proceeding
        cont.Enable();
        continuePressed = false;
        promptText.gameObject.SetActive(true);
        promptText.CrossFadeAlpha(1, 0.5f, true);
        yield return new WaitUntil(() => continuePressed);

        // once continue is pressed, turn off cutscene manager
        promptText.CrossFadeAlpha(0, 0.5f, true);
        videoRenderImg.enabled = false;
        
        // Disable cutscene controls and reenable gameplay. 
        playerControls.Disable();
        playerControls.PlayerGameplay.Enable();
        playerControls.PlayerGameplay.Pause.Disable();
        playerControls.PlayerGameplay.Interact.Disable();
        
        onVideoFinishEvents?.Invoke();

        // By now, video has stopped, close screen
        yield return StartCoroutine(LoadScreen(false, fadeOutTime));
        promptText.gameObject.SetActive(false);

        onCutsceneFadeFinishEvents?.Invoke();

        // Disable pausing just to be safe in big prevention
        playerControls.PlayerGameplay.Pause.Enable();
        playerControls.PlayerGameplay.Interact.Enable();

        // wipe both sets of events clear
        onVideoFinishEvents = null;
        onCutsceneFadeFinishEvents = null;
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
        float t = 0;
        while (t < dur)
        {
            background.color = Color.Lerp(originalCol, targetCol, (t / dur));
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        // make sure the target color is reached
        background.color = targetCol;

        yield return null;
    }

    /// <summary>
    /// submit request to continue the cutscene
    /// </summary>
    /// <param name="c"></param>
    private void ContinueCutscene(InputAction.CallbackContext c)
    {
        continuePressed= true;
    }

    #endregion

    /// <summary>
    /// Skip cutscene to the very end
    /// </summary>
    /// <param name="c"></param>
    public void SkipCutscene(InputAction.CallbackContext c = default)
    {
        // skip video forward to end
        fastForward.Disable();
        // include offset as if you go over, the time will go to 0 instead
        videoPlayer.time = videoPlayer.length - 0.5f;
    }
}
