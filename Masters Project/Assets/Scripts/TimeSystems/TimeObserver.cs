using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface TimeObserver
{
    public abstract void OnStop();

    public abstract void OnResume();
}
