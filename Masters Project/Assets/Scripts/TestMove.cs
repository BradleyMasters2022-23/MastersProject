using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMove : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.forward * 3f * TimeManager.WorldDeltaTime;

    }
}
