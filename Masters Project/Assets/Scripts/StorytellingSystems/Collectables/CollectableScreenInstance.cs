using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableScreenInstance : MonoBehaviour
{
    public static CollectableScreenInstance instance;
    private CollectableViewUI ui;
    public CollectableViewUI UI { get { return ui; } }
    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            ui = GetComponentInChildren<CollectableViewUI>(true);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
