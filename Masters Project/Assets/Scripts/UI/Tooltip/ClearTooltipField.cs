using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearTooltipField : MonoBehaviour
{
    public void ClearTooltip()
    {
        if (TooltipManager.instance != null)
        {
            TooltipManager.instance.UnloadTooltip();
        }
    }
}
