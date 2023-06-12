using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallManagerSecret : ConversationInteract
{
    [Header("Secrets")]

    [Tooltip("How much stronger than the current depth should the crystal spawn at")]
    [SerializeField] private int crystalBuff;
    [SerializeField] private CrystalInteract crystal;

    [SerializeField] private Animator anim;

    private bool available = true;

    public override void OnInteract()
    {
        if (!available) return;

        base.OnInteract();
        StartCoroutine(CheckForComplete());
    }

    public override bool CanInteract()
    {
        return available;
    }

    /// <summary>
    /// Continually check until a conversation was complete
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckForComplete()
    {
        int originalCnt = CallManager.instance.AvailableCallCount();
        Debug.Log($"Available count set to {originalCnt}, now waiting");
        // Wait until an available conversation was removed to kill the terminal
        yield return new WaitUntil(()=> originalCnt != CallManager.instance.AvailableCallCount());

        available = false;
        SpawnNewCrystal();
        SetOffline();
    }

    /// <summary>
    /// Spawn a new crystal at a stronger level than normal
    /// </summary>
    private void SpawnNewCrystal()
    {
        crystal.RandomizeCrystalSetLevel(MapLoader.instance.PortalDepth() + crystalBuff);
        crystal.gameObject.SetActive(true);
    }
    /// <summary>
    /// Set the terminal to offline, handling visuals and functionality
    /// </summary>
    private void SetOffline()
    {
        // turn off the interact
        ConvoRefManager.instance.CloseAllScreens();
        //this.enabled = false;

        // set the animator to play through the sequence of offline mode
        anim?.SetTrigger("play");
    }
}
