using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AlertUIManager : MonoBehaviour
{
    /// <summary>
    /// global reference for alert UI display
    /// </summary>
    public static RectTransform display;

    private void Awake()
    {
        display = GetComponent<RectTransform>();
    }
}
