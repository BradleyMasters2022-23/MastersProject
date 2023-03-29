using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TooltipHolder : MonoBehaviour
{
    public TooltipSO tooltip;
    private TooltipManager manager;

    private void Start()
    {
        if (tooltip == null)
        {
            Destroy(this);
            return;
        }
        manager = TooltipManager.instance;
    }

    public void SubmitTooltip()
    {
        manager?.RequestTooltip(tooltip);
    }

    public void RetractTooltip()
    {
        manager?.UnloadTooltip(tooltip);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            SubmitTooltip();
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
            RetractTooltip();
    }
}
