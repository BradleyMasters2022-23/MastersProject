using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FullNoteCompiler : MonoBehaviour
{
    private NoteObject note;

    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI fragmentCounter;
    [SerializeField] private TextMeshProUGUI noteData;

    [SerializeField] private string noNoteFiller;

    public void LoadNote(NoteObject n)
    {
        // get data, set name
        note = n;
        title.text = n.displayName;
        noteData.text = "";

        // Load collected fragments, filling gaps. Also track fragment found
        List<Fragment> allFrags = new List<Fragment>(note.GetFragments());
        int collected = 0;

        for (int i = 0; i < allFrags.Count; i++)
        {
            if (note.FragmentFound(allFrags[i]))
            {
                noteData.text += allFrags[i].content;
                collected++;
            }
            else
            {
                noteData.text += "<br><br>";
                noteData.text += noNoteFiller;
                noteData.text += "<br><br>";
            }
        }

        fragmentCounter.text = collected.ToString() + " / " + allFrags.Count + " fragments found";

    }

    private void OnDisable()
    {
        ClearUI();
    }

    public void ClearUI()
    {
        note = null;
        title.text = "";
        fragmentCounter.text = "";
        noteData.text = "";
        gameObject.SetActive(false);
    }
}
