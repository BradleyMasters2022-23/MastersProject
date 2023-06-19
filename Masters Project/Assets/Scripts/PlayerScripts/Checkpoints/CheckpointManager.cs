using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;
    [SerializeField] private ChannelVoid playerAimChannel;
    [SerializeField] Transform teleportLocation;
    private PlayerTarget player;
    private bool movingPlayer;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
    }
    /// <summary>
    /// Get player ref on start
    /// </summary>
    private void Start()
    {
        player = PlayerTarget.p;
    }
    /// <summary>
    /// Load a new checkpoint to return to
    /// </summary>
    /// <param name="t">New checkpoint. Make sure its a stationary transform</param>
    public void SetNewCheckpoint(Transform t)
    {
        teleportLocation = t;
    }

    /// <summary>
    /// Send the player to the currently stored checkpoint
    /// </summary>
    public void SendPlayerToCheckpoint()
    {
        if(!movingPlayer)
            StartCoroutine(MovePlayer());
        
    }

    /// <summary>
    /// Move player to the checkpoint, managing controls AND UI at the same time
    /// </summary>
    /// <returns></returns>
    private IEnumerator MovePlayer()
    {
        //Debug.Log("Moving player");
        movingPlayer = true;

        if (player != null && teleportLocation != null)
        {
            // Stop player control, begin fade
            GameManager.controls.PlayerGameplay.Disable();
            yield return HUDFadeManager.instance.FadeIn();

            // Teleport player to checkpoint location, reset their aim
            player.transform.position = teleportLocation.position;
            player.transform.rotation = teleportLocation.rotation;
            playerAimChannel?.RaiseEvent();
            yield return new WaitForSecondsRealtime(0.3f);


            // Undo fade, enable controls
            yield return HUDFadeManager.instance.FadeOut();
            GameManager.controls.PlayerGameplay.Enable();
        }
        else
        {
            Debug.Log("Tried to send player to checkpoint, but no player found!");
        }
        
        movingPlayer = false;
        yield return null;
    }

}
