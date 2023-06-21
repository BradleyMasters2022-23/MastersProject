/* ================================================================================================
 * Author - Ben Schuster   
 * Date Created - June 15th, 2023
 * Last Edited - June 15th, 2023 by Ben Schuster
 * Description - Handles loading tooltip data into UI
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TooltipUIDataLoader : MonoBehaviour
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text data;
    [SerializeField] private Image img;
    /// <summary>
    /// The data currently loaded into the object
    /// </summary>
    private TooltipSO loadedData;

    public delegate void OnPress(TooltipSO data);
    private OnPress onPress;

    /// <summary>
    /// Populate this UI element with relevant data
    /// </summary>
    /// <param name="d">Data to implement</param>
    /// <param name="pressExecute">Function to execute on button press</param>
    public void LoadInData(TooltipSO d, OnPress pressExecute = null)
    {
        onPress = pressExecute;
        loadedData= d;

        // If invalid data passed in, clear
        if(loadedData == null)
        {
            ResetFields();
            return;
        }

        if (title != null)
            title.text = d.titleText;
        if (data != null)
            data.text = d.tooltipText;
       // if(img != null)
       //     img.sprite = d.icon
    }

    /// <summary>
    /// Reset all fields  for this display
    /// </summary>
    public void ResetFields()
    {
        onPress = null;
        loadedData = null;

        if (title != null)
            title.text = "";
        if (data != null)
            data.text = "";
        // if(img != null)
        //     img.sprite = d.icon
    }

    /// <summary>
    /// When the button is pressed, invoke the passed in action if any
    /// </summary>
    public void OnButtonPress()
    {
        onPress?.Invoke(loadedData);
    }
}
