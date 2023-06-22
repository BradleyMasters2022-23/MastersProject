/* ================================================================================================
 * Author - Ben Schuster   
 * Date Created - June 15th, 2023
 * Last Edited - June 15th, 2023 by Ben Schuster
 * Description - Handles tooltip history UI
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class TooltipUIListLoader : MonoBehaviour
{
    [Tooltip("All tooltips that should be viewable in this screen")]
    [SerializeField] private TooltipSO[] allTooltips;
    /// <summary>
    /// All tooltips found that met conditions to be displayed
    /// Current conditions - Found atleast once
    /// </summary>
    private List<TooltipSO> foundTooltips;

    [Space(5),Tooltip("Prefab of an individual list item")]
    [SerializeField] private GameObject tooltipSectionPrefab;
    [Tooltip("Area to load tooltip list items into")]
    [SerializeField] private RectTransform displayArea;
    [SerializeField] private Scrollbar scroll;
    /// <summary>
    /// Reference to all spawned in objects
    /// </summary>
    private GameObject[] spawnedObjects;

    [Space(5), Tooltip("The expanded view for when a tooltip option is clicked")]
    [SerializeField] private TooltipUIDataLoader expandedView;

    /// <summary>
    /// Handle preparing the UI
    /// </summary>
    private void OnEnable()
    {
        foundTooltips = GetLoadableTooltips();
        spawnedObjects = PopulateList(displayArea, foundTooltips.ToList());

    }
    /// <summary>
    /// Handle resetting the UI
    /// </summary>
    private void OnDisable()
    {
        // reset expanded fields
        expandedView.gameObject.SetActive(false);
        expandedView.ResetFields();

        // Clear all data loaded in at start
        foundTooltips = null;
        foreach(var o in spawnedObjects)
        {
            Destroy(o.gameObject);
        }
    }

    /// <summary>
    /// Get all tooltips to load
    /// </summary>
    /// <returns>List of all found tooltips</returns>
    private List<TooltipSO> GetLoadableTooltips()
    {
        // Prepare data
        List<TooltipSO> found = new List<TooltipSO>();
        TooltipSaveData data = TooltipManager.instance.GetSaveData();

        // Check each tooltip to see if its been found atleast once
        foreach (var tp in allTooltips)
        {
            if (tp.showInTooltipList && data.HasTooltip(tp))
                found.Add(tp);
        }

        return found;
    }

    /// <summary>
    /// Populate the target with all found tooltips
    /// </summary>
    /// <param name="target">Target to populate items into</param>
    /// <returns>Reference to all objects</returns>
    private GameObject[] PopulateList(RectTransform target, List<TooltipSO> data)
    {
        // prepare list
        GameObject[] spawned = new GameObject[data.Count];

        // Load in an object for each data object
        for (int i = 0; i < spawned.Length; i++)
        {
            GameObject o = Instantiate(tooltipSectionPrefab, target);
            spawned[i] = o;
            o.GetComponent<TooltipUIDataLoader>().LoadInData(data[i], LoadExpandedView);
        }

        target.GetComponent<SmartExpandContent>()?.CalculateHeight();
        if (scroll != null)
            scroll.value = 1;
            
        // return reference to all objects
        return spawned;
    }

    /// <summary>
    /// Open the expanded view of a tooltip. Passed into options as delagate
    /// </summary>
    public void LoadExpandedView(TooltipSO d)
    {
        expandedView.gameObject.SetActive(true);
        expandedView.LoadInData(d);
    }
}
