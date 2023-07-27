using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearInitializer : RoomInitializer
{
    [Header("Linear Room Loading")]
    [Tooltip("Which fields are required to be completed to unlock this room")]
    [SerializeField] private SpawnTriggerField[] combatFields;
    private bool unlocked;

    // Start is called before the first frame update
    public override void Init(Cubemap nextCubemap, float intensity)
    {
        // init all fields
        foreach (var field in combatFields)
        {
            field.Init();
        }

        base.Init(nextCubemap, intensity);

        // tell each one to load their waves 
        foreach (var field in combatFields)
        {
            field.LoadWaves();
        }

        unlocked = false;
    }

    // Update is called once per frame
    void Update()
    {
        // for testing, if no combat fields, instantly unlock
        if(!unlocked && combatFields.Length == 0)
        {
            MapLoader.instance.EndRoomEncounter();
            if (BackgroundMusicManager.instance != null)
                BackgroundMusicManager.instance.SetMusic(Music.NonCombat, 2f);
            unlocked = true;
        }

        // Check if the required fields are finished
        if (!unlocked)
        {
            // check if all combat fields are finished yet
            bool finished = true;
            foreach (var field in combatFields)
            {
                if (!field.Finished)
                {
                    finished = false;
                    break;
                }
            }

            // fade back to non combat music
            if (finished)
            {
                MapLoader.instance.EndRoomEncounter();
                if (BackgroundMusicManager.instance != null)
                    BackgroundMusicManager.instance.SetMusic(Music.NonCombat, 2f);

                unlocked = true;
            }

        }
    }
}
