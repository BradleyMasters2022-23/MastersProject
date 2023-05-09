using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Video;

public class CutsceneTrigger : MonoBehaviour
{
    [Tooltip("Cutscene manager to trigger")]
    [SerializeField] CutsceneManager cutscenePlayer;
    [Tooltip("Cutscene to play")]
    [SerializeField] VideoClip cutscene;
    [Tooltip("Whether or not this cutscene should only be played once per save")]
    [SerializeField] private bool onlyPlayOnce;
    [Tooltip("Whether or not this trigger is playable")]
    [SerializeField, ReadOnly] private bool playable = false;

    /// <summary>
    /// Reference to video player obj
    /// </summary>
    private VideoPlayer player;

    private const string fileName = "cutsceneSaveData";
    private CutsceneSaveData saveData;

    protected virtual void Awake()
    {
        // get saved data
        saveData = DataManager.instance.Load<CutsceneSaveData>(fileName);
        if(saveData == null)
            saveData = new CutsceneSaveData();

        // If this should only be playable once, check if it was already saved
        // Otherwise, let it play
        if (onlyPlayOnce)
            playable = !saveData.SinglePlayOnly(cutscene);
        else
            playable = true;
    }

    private void OnEnable()
    {
        // prepare the cutscene ASAP
        cutscenePlayer.PrepareCutscene(cutscene);
    }

    /// <summary>
    /// Try to play the cutscene based on internal parameters
    /// </summary>
    public void TryCutscene()
    {
        
        if(CanPlay())
        {
            Debug.Log($"Trying to play cutscene {cutscene.name}");
            playable = true;
            StartCoroutine(LoadCutscene());
        }
    }

    /// <summary>
    /// Load up the cutscene and play it once its prepared 
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadCutscene()
    {
        Debug.Log("Preparing cutscene");

        player = cutscenePlayer.GetComponent<VideoPlayer>();
        if (player == null || cutscene == null)
            yield break;

        // Get most recent save data and update it
        saveData = DataManager.instance.Load<CutsceneSaveData>(fileName);
        if(saveData == null)
            saveData = new CutsceneSaveData();
        saveData.PlayCutscene(cutscene, onlyPlayOnce);
        DataManager.instance.Save(fileName, saveData);

        Debug.Log("Waiting to prepare");

        // make sure player is prepared
        yield return new WaitUntil(() => player.isPrepared);

        Debug.Log("Preperation done, starting cutscene");

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
