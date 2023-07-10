using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamTooltipTrigger : TooltipHolder
{
    public static SamTooltipTrigger instance;
    [SerializeField] private Fragment frag;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public void TriggerTooltip()
    {
        SubmitTooltip();
    }
}
