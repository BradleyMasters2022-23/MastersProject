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

[CreateAssetMenu(menuName = "Gameplay/Fragment Data")]
public class Fragment : ScriptableObject
{
    [Tooltip("ID of this fragment within its note.")]
    public int fragmentID;

    [Tooltip("ID of the note this fragment belongs to.")]
    public int noteID;

    [Tooltip("Content of this fragment.")]
    public string content;

    [Tooltip("Whether or not this note has been found.")]
    public bool found;
}
