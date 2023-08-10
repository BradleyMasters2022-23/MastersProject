using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalRoomLoader : RoomInitializer
{
    [SerializeField] List<GameObject> stuffToDisable;

    private void Awake()
    {
        if (MapLoader.instance == null)
            Init(null, 1);
    }

    public override void Init(Cubemap cubemap, float intensity)
    {
        //Debug.Log("Boss room init");
        base.Init(cubemap, intensity);
        foreach(var obj in stuffToDisable)
        {
            obj.SetActive(false);
        }
    }
}
