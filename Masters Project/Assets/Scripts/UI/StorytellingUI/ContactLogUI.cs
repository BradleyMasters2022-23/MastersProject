/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - June 9th, 2023
 * Last Edited - June 9th, 2023 by Ben Schuster
 * Description - Manages the 'call dots' on the UI
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ContactLogUI : MonoBehaviour
{
    private Conversation convoRef;
    private Image mImage;
    [SerializeField] private Sprite uncheckedSprite;
    [SerializeField] private Sprite checkedSprite;

    [SerializeField] private float flashInterval;
    [SerializeField] private Color flashColor1;
    [SerializeField] private Color flashColor2;
    
    #region Convo Status

    /// <summary>
    /// Load a conversation into this log option
    /// </summary>
    /// <param name="c"></param>
    public void SetConvo(Conversation c)
    {
        mImage = GetComponent<Image>();
        convoRef = c;
    }
    /// <summary>
    /// Check status of the loaded conversation
    /// </summary>
    public void CheckConvoStatus()
    {
        if (convoRef == null) return;

        if (CallManager.instance.HasNewCall(convoRef))
        {
            StartFlashing();
        }
        else if (CallManager.instance.CallInSave(convoRef))
        {
            SetFound();
        }
        else
        {
            SetNotFound();
        }
    }

    private void OnEnable()
    {
        CheckConvoStatus();
    }

    #endregion

    #region Status Functionality

    private void SetFound()
    {
        mImage.sprite = checkedSprite;
    }
    private void SetNotFound()
    {
        mImage.sprite = uncheckedSprite;
    }
    
    private void StartFlashing()
    {
        StartCoroutine(FlashRoutine());
    }
    private IEnumerator FlashRoutine()
    {
        mImage.color = flashColor1;
        mImage.sprite = checkedSprite;

        Color currColor = flashColor1;
        while (true)
        {
            yield return new WaitForSecondsRealtime(flashInterval);

            currColor = (currColor == flashColor1) ? flashColor2 : flashColor1;
            mImage.color = currColor;
            yield return null;
        }
    }
    private void StopFlashing()
    {
        StopAllCoroutines();
    }

    #endregion
}
