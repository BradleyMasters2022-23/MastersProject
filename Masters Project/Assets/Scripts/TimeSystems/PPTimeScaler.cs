using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PPTimeScaler : MonoBehaviour
{
    private Volume renderingVolume;

    // Start is called before the first frame update
    void Start()
    {
        renderingVolume = GetComponent<Volume>();
        if (renderingVolume == null)
            Destroy(this);
    }

    // Update is called once per frame
    void Update()
    {
        renderingVolume.weight = 1 - TimeManager.WorldTimeScale;
    }
}
