using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalRoomLoader : RoomInitializer
{
    [SerializeField] List<GameObject> stuffToDisable;

    private void Awake()
    {
        if (MapLoader.instance == null)
            Init();
    }

    public override void Init()
    {
        Debug.Log("Boss room init");
        base.Init();
        foreach(var obj in stuffToDisable)
        {
            obj.SetActive(false);
        }
    }
}
