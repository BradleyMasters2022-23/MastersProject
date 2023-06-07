/* ================================================================================================
 * Author - Soma Hannon
 * Date Created - October 25, 2022
 * Last Edited - October 31, 2022 by Soma Hannon
 * Description - Defines a single fragment of a NoteObject.
 * ================================================================================================
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Storytelling/Fragment Data")]
public class Fragment : ScriptableObject
{
    [Tooltip("ID of this fragment within its note.")]
    public int fragmentID;

    [Tooltip("ID of the note this fragment belongs to.")]
    public int noteID;

    [Tooltip("Content of this fragment."), TextArea(15, 50)]
    public string content;

    //[Tooltip("Whether or not this fragment has been found.")]
    //public bool found = false;

    public string GetNoteName()
    {
       return AllNotesManager.instance.GetNote(noteID).displayName;
    }

    public int GetFragmentID() 
    { 
        return fragmentID; 
    }
}
