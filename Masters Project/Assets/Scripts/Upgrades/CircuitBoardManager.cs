using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircuitBoardManager : MonoBehaviour
{
    public PlayerController player;
    public List<CircuitObject> circuits = new List<CircuitObject>();
    public static CircuitBoardManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            CircuitBoardManager.instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }

        foreach(CircuitObject circuit in circuits)
        {

        }
    }

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }

    /// <summary>
    /// initializes circuit and attaches it to player
    /// </summary>
    private void InitializeCircuit(CircuitObject co) {
        ICircuit temp = Instantiate(co.circuitPrefab, player.gameObject.transform).GetComponent<ICircuit>();
        temp.LoadCircuit(player);
    }

    public void UnlockCircuit(int index)
    {
        if(circuits[index].Unlockable())
        {
          circuits[index].unlocked = true;
          InitializeCircuit(circuits[index]);
        }
    }
}
