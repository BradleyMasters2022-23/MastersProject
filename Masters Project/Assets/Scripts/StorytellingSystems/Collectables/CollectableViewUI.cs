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
    private CollectableSO loadedCollectable;
    [Tooltip("All fragment indexes to load"), SerializeField]
    private List<int> fragmentsToLoad;
    [Tooltip("Spawnpoint for collectable props in UI"), SerializeField]
    private Transform objectSpawnpoint;
    [Tooltip("Textbox that displays description"), SerializeField]
    private TextMeshProUGUI descTextbox;
    [Tooltip("Camera that renders this UI"), SerializeField]
    private Camera renderCam;

    /// <summary>
    /// List of props spawned
    /// </summary>
    private List<GameObject> spawnedPropList;
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
        if(spawnedPropList == null)
            spawnedPropList = new List<GameObject>();
        else
            spawnedPropList.Clear();

        // Spawn in each fragment passed in
        loadedCollectable = coreData;
        fragmentsToLoad = fragments;
        foreach (var fragment in fragments)
        {
            CollectableFragment f = loadedCollectable.GetFragment(fragment);
            GameObject cont = Instantiate(f.ObjectPrefab, objectSpawnpoint);
            cont.gameObject.transform.localScale *= f.PropUIScaleMod;

            // set prop's children to UI so it renders correctly
            foreach(var part in cont.GetComponentsInChildren<Transform>())
                part.gameObject.layer = LayerMask.NameToLayer("UI");

            spawnedPropList.Add(cont);
        }
        descTextbox.text = loadedCollectable.GetDesc(true, fragments);

        Camera.main.GetUniversalAdditionalCameraData().cameraStack.Add(renderCam);

        // Open the screen
        front = true;
        gameObject.SetActive(true);
        GameManager.instance.ChangeState(GameManager.States.GAMEMENU);
    }

    public void OpenUI(CollectableSO coreData, int fragment)
    {
        OpenUI(coreData, new List<int>() { fragment });
    }

    /// <summary>
    /// Reset UI to base state
    /// </summary>
    public void ResetUI()
    {
        front = true;
        descTextbox.text = "";
        Camera.main.GetUniversalAdditionalCameraData().cameraStack.Remove(renderCam);

        foreach (var obj in spawnedPropList.ToArray())
        {
            if(obj != null)
                Destroy(obj);
        }

        loadedCollectable = null;
        fragmentsToLoad = null;

        spawnedPropList.Clear();
    }

    /// <summary>
    /// flip the front, reload description text
    /// </summary>
    public void Flip()
    {
        front = !front;
        SwapData();
    }

    /// <summary>
    /// Swap data between normal and alt. Called via animator
    /// </summary>
    public void SwapData()
    {
        descTextbox.text = loadedCollectable.GetDesc(front, fragmentsToLoad);
    }
}
