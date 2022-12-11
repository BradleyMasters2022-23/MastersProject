using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.CompilerServices;

public class UnpackNoteUI : MonoBehaviour
{
    [SerializeField] private NoteObject note;
    [SerializeField] private TextMeshProUGUI nameTextbox;
    [SerializeField] private TextMeshProUGUI fragmentCounterText;

    [SerializeField] private FullNoteCompiler targetDisplay;

    public void InitializeNoteUI(NoteObject n)
    {
        note = n;
        nameTextbox.text = n.displayName;

        List<Fragment> allFrags = new List<Fragment>(note.GetFragments());
        int collected = 0;

        for(int i = 0; i < allFrags.Count; i++)
        {
            if (allFrags[i].found)
                collected++;
        }

        fragmentCounterText.text = collected.ToString() + " / " + allFrags.Count + " fragments found";
    }

    private void OnDisable()
    {
        ClearUI();
    }

    public void ClearUI()
    {
        note = null;
        nameTextbox.text = "";
        fragmentCounterText.text = "";
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Open the target screen and load in the assigned note!
    /// </summary>
    public void SelectNote()
    {
        targetDisplay.LoadNote(note);
        targetDisplay.gameObject.SetActive(true);
    }
}
