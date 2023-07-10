using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Video;
using UnityEngine.Events;

public class CutsceneTrigger : MonoBehaviour
{
    [Tooltip("Cutscene manager to trigger")]
    [SerializeField, ReadOnly] CutsceneManager cutscenePlayer;
    [Tooltip("Cutscene to play")]
    [SerializeField] VideoClip cutscene;
    [Tooltip("Whether or not this cutscene should only be played once per save")]
    [SerializeField] protected bool onlyPlayOnce;
    [Tooltip("Whether or not this trigger is playable")]
    [SerializeField, ReadOnly] private bool playable = false;

    [Tooltip("events that execute once the video itself finishes")]
    [SerializeField] protected UnityEvent onVideoFinishEvents;
    [Tooltip("events that execute once the cutscene is finished and the game fades back into the game")]
    [SerializeField] protected UnityEvent onCutsceneFadeFinishEvents;

    private const string fileName = "cutsceneSaveData";
    private CutsceneSaveData saveData;

    protected virtual void Awake()
    {
        if(cutscene == null)
        {
            enabled = false;
            return;
        }

        // get saved data
        saveData = DataManager.instance.Load<CutsceneSaveData>(fileName);
        if(saveData == null)
            saveData = new CutsceneSaveData();

        // If this should only be playable once, check if it was already saved
        // Otherwise, let it play
        if (onlyPlayOnce)
        {
            bool inSafe = saveData.SinglePlayOnly(cutscene);
            //Debug.Log($"Only play once enabled | In save data {inSafe} | Setting playable to {!inSafe}");
            playable = !inSafe;
        }
        else
            playable = true;
    }

    /// <summary>
    /// Try to play the cutscene based on internal parameters
    /// </summary>
    public void TryCutscene()
    {
        if (CanPlay())
        {
            playable = true;
            StartCoroutine(PlayRoutine());
            
        }
    }

    /// <summary>
    /// Routine that initializes and loads the cutscene
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayRoutine()
    {
        yield return StartCoroutine(InitCutscene());
        yield return StartCoroutine(LoadCutscene());
    }
    /// <summary>
    /// Pass the video into the manager to load
    /// </summary>
    /// <returns></returns>
    private IEnumerator InitCutscene()
    {
        yield return new WaitUntil(()=> CutsceneManager.instance != null);
        cutscenePlayer = CutsceneManager.instance;
        cutscenePlayer.PrepareCutscene(cutscene);
    }
    /// <summary>
    /// Load up the cutscene and play it once its prepared 
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadCutscene()
    {
        if (cutscenePlayer == null || cutscene == null)
            yield break;


        // Get most recent save data and update it
        saveData = DataManager.instance.Load<CutsceneSaveData>(fileName);

        if(saveData == null)
            saveData = new CutsceneSaveData();

        saveData.PlayCutscene(cutscene, onlyPlayOnce);
        bool s = DataManager.instance.Save(fileName, saveData);

        // Load any events, if there are any
        if (onVideoFinishEvents.GetPersistentEventCount() > 0)
            cutscenePlayer.LoadVideoEndEvents(onVideoFinishEvents);
        if(onCutsceneFadeFinishEvents.GetPersistentEventCount() > 0)
            cutscenePlayer.LoadCutsceneEndEvents(onCutsceneFadeFinishEvents);

        // if there is a map loader, wait for that to finish loading as well
        if (MapLoader.instance != null && MapLoader.instance.LoadState == LoadState.Loading)
        {
            yield return new WaitUntil(() => MapLoader.instance.LoadState != LoadState.Loading);
            yield return new WaitForSecondsRealtime(0.5f);
            if(GameManager.instance.CurrentState == GameManager.States.PAUSED)
            {
                GameManager.instance.CloseToTop();
            }
                
        }

        // wait until its done preparing
        yield return new WaitUntil(() => cutscenePlayer.GetComponent<VideoPlayer>().isPrepared);

        // Play cutscene
        cutscenePlayer.BeginCutscene();
        yield return null;

    }

    /// <summary>
    /// Whether or not this cutscene can be played
    /// </summary>
    /// <returns></returns>
    protected virtual bool CanPlay()
    {
        return playable;
    }
}
