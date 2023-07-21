/* ================================================================================================
 * Author - Ben Schuster   
 * Date Created - July 18th, 2023
 * Last Edited - July 198th, 2023 by Ben Schuster
 * Description - UI that loads collectable views
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.Universal;

public class CollectableViewUI : MonoBehaviour
{
    /// <summary>
    /// the loaded collectable
    /// </summary>
    private CollectableSO loadedCollectable;
    /// <summary>
    /// All fragment indexes to load
    /// </summary>
    [SerializeField] private List<int> fragmentsToLoad = new List<int>();
    [Tooltip("Spawnpoint for collectable props in UI"), SerializeField]
    private Transform objectSpawnpoint;
    [Tooltip("Textbox that displays title"), SerializeField]
    private TextMeshProUGUI titleTextBox;
    [Tooltip("Textbox that displays description"), SerializeField]
    private TextMeshProUGUI descTextbox;
    [Tooltip("The display area tracking whether prop is front or back"), SerializeField] 
    private PropScrollArea propDisplayArea;

    /// <summary>
    /// List of props spawned
    /// </summary>
    private GameObject spawnedProp;
    /// <summary>
    /// Whether the front is currently displayed
    /// </summary>
    private bool front;

    /// <summary>
    /// Load data into UI, open it
    /// </summary>
    /// <param name="coreData">reference to collectable being loaded</param>
    /// <param name="fragments">fragment indexes to load</param>
    public void OpenUI(CollectableSO coreData, List<int> fragments)
    {
        // Set data
        loadedCollectable = coreData;
        fragmentsToLoad = fragments;

        // spawn in prop, set scale
        spawnedProp = Instantiate(loadedCollectable.Prop(), objectSpawnpoint);
        spawnedProp.gameObject.transform.localScale *= loadedCollectable.PropUIScaleMod;

        // convert list of fragments to indexes, excluding repeats
        List<int> targetChildIndexs = new List<int>();
        foreach(int i in fragmentsToLoad)
        {
            int tgt = loadedCollectable.GetFragment(i).PropIndex;
            if(!targetChildIndexs.Contains(tgt))
                targetChildIndexs.Add(tgt);
        }

        // enable/disable fragments not found, if they exist
        for (int i = 0; i < spawnedProp.transform.childCount; i++)
        {
            spawnedProp.transform.GetChild(i).gameObject.SetActive(targetChildIndexs.Contains(i) );
        }

        // set prop's children to UI so it renders correctly
        foreach (var part in spawnedProp.GetComponentsInChildren<Transform>())
            part.gameObject.layer = LayerMask.NameToLayer("UI");

        // load text data
        titleTextBox.text = loadedCollectable.Name();
        SwapData();

        // if theres a flip description, subsctibe to it 
        if (coreData.FlipDescription)
            propDisplayArea.SubscribeToFlip(Flip);

        // Open the screen
        front = true;
        gameObject.SetActive(true);
        GameManager.instance.ChangeState(GameManager.States.GAMEMENU);
    }

    /// <summary>
    /// Load a single fragment into the UI
    /// </summary>
    /// <param name="coreData">refernece to collectable being loaded</param>
    /// <param name="fragment">fragment index to load</param>
    public void OpenUI(CollectableSO coreData, int fragment)
    {
        OpenUI(coreData, new List<int>() { fragment });
    }

    /// <summary>
    /// Reset UI to base state
    /// </summary>
    public void ResetUI()
    {
        // reset specific UI screen
        front = true;
        descTextbox.text = "";
        titleTextBox.text = "";

        // destroy prop if possible
        if (spawnedProp != null)
            Destroy(spawnedProp);

        // if flip desc set, make sure to unsub
        if (loadedCollectable != null && 
            loadedCollectable.FlipDescription)
            propDisplayArea.UnSubscribeToFlip(Flip);

        StopAllCoroutines();
        descTextbox.alpha = 1;

        loadedCollectable = null;
        fragmentsToLoad = null;
    }

    /// <summary>
    /// flip the front, reload description text if set
    /// </summary>
    public void Flip(bool newFront)
    {
        if (front == newFront) return;

        front = newFront;
        StopAllCoroutines();
        StartCoroutine(SwapText());
    }

    /// <summary>
    /// Swap data between normal and alt
    /// </summary>
    private void SwapData()
    {
        if (fragmentsToLoad.Count > 1)
            descTextbox.text = loadedCollectable.GetDesc(front, fragmentsToLoad);
        else
            descTextbox.text = loadedCollectable.GetDesc(front, fragmentsToLoad[0]);
    }

    /// <summary>
    /// Swap the text out for the relevant side
    /// </summary>
    /// <returns></returns>
    private IEnumerator SwapText()
    {
        descTextbox.CrossFadeAlpha(0, .3f, true);
        yield return new WaitForSecondsRealtime(.6f);
        SwapData();
        descTextbox.CrossFadeAlpha(1, .3f, true);
        yield return new WaitForSecondsRealtime(.3f);
    }
}
