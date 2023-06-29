using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BFLTarget : Target
{
    [SerializeField] float recoveryTime;
    [SerializeField] BFLController controller;
    private LocalTimer recoveryTracker;
    private Coroutine recoveryRoutine;

    [SerializeField] GameObject healthbar;

    /// <summary>
    /// When phase begins, make sure to reset the BFL to prevent bugs
    /// </summary>
    public void NewPhase()
    {
        if(recoveryRoutine != null)
        {
            //Debug.Log("New phase called during recovery");
            StopCoroutine(recoveryRoutine);
            _healthManager.ResetHealth();
            _healthManager.ToggleGodmode(false);
            _killed = false;
        }
    }

    /// <summary>
    /// Instead of killing the BFL, disable it and begin recovery phase
    /// </summary>
    protected override void KillTarget()
    {
        //Debug.Log($"{name} has been killed and thus inturrupted");

        //if (_killed) return;

        _killed = true;

        controller?.Inturrupt();
        controller?.DisableBFL();
        _healthManager.ToggleGodmode(true);
        recoveryRoutine = StartCoroutine(Recover());
    }

    /// <summary>
    /// Regenerate the BFL over time. Later try lerping health
    /// </summary>
    /// <returns></returns>
    private IEnumerator Recover()
    {
        //Debug.Log("Recovery started");
        if (recoveryTracker != null)
        {
            recoveryTracker.ResetTimer();
        }
        else
            recoveryTracker = GetTimer(recoveryTime);

        yield return new WaitUntil(recoveryTracker.TimerDone);

        //Debug.Log("Recovery finished");

        _healthManager.ResetHealth();
        _healthManager.ToggleGodmode(false);
        _killed = false;

        controller?.EnableBFL();
    }

    public void SetHealthbarStatus(bool enabled)
    {
        _healthManager.ToggleGodmode(!enabled);
        healthbar.SetActive(enabled);
    }
}
