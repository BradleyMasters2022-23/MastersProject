using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalRoomLoader : RoomInitializer
{
    [SerializeField] List<GameObject> stuffToDisable;

    public override void Init()
    {
        base.Init();
        foreach(var obj in stuffToDisable)
        {
            obj.SetActive(false);
        }
    }
}
