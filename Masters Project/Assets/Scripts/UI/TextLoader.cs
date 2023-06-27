/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - June 26th, 2023
 * Last Edited - June 26th, 2023
 * Description - Handles loading text into attached text ref
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshPro))]
public class TextLoader : MonoBehaviour
{
    private TextMeshPro _textMeshPro;


    [SerializeField] float charLoadTime;

    private void OnEnable()
    {
        _textMeshPro = GetComponent<TextMeshPro>();
    }
}
