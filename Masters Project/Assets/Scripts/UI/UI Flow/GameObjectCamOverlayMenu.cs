using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GameObjectCamOverlayMenu : GameObjectMenu
{
    [Tooltip("Camera that renders this UI"), SerializeField]
    private Camera renderCam;

    /// <summary>
    /// When opening screen, disable default overlay UI
    /// </summary>
    public override void OpenMenu()
    {
        Camera.main.GetUniversalAdditionalCameraData().cameraStack.Add(renderCam);
        PlayerTarget.p.MainUI.enabled = false;

        base.OpenMenu();
    }

    /// <summary>
    /// when closing, make sure to re-enable the original canvas UI
    /// </summary>
    public override void CloseFunctionality()
    {
        // revert 
        Camera.main.GetUniversalAdditionalCameraData().cameraStack.Remove(renderCam);
        PlayerTarget.p.MainUI.enabled = true;

        base.CloseFunctionality();
    }
}
