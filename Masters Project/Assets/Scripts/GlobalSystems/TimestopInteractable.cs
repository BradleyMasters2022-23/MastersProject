using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimestopInteractable : MonoBehaviour, TimeObserver
{
    [SerializeField] private bool showInNormalTime;
    [SerializeField] private bool showInStoppedTime;

    private void Awake()
    {
        TimeManager.instance.Subscribe(this);

        if (TimeManager.TimeStopped)
            OnStop();
        else
            OnResume();
    }

    public void OnResume()
    {
        gameObject.SetActive(showInNormalTime);
    }

    public void OnStop()
    {
        gameObject.SetActive(showInStoppedTime);
    }

    private void OnDestroy()
    {
        TimeManager.instance.UnSubscribe(this);
    }
}
