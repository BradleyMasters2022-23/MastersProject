using DynamicLightmaps;
using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;

public class LightStateManager : LayoutRandomizer
{
    [Tooltip("All light states that ")]
    [SerializeField] private LightState[] lightStates;

    [SerializeField, ReadOnly] private int index = -1;
    [SerializeField] private bool testingLight;

    private LightState currentState;

    protected override void Start()
    {
        base.Start();

        // NOTE IF YOU WANT TO CHANGE THE STATE OF THE LIGHTMAPS AT THE BEGINING OF THE GAME
        // CHANGE IT IN START NOT AWAKE BECAUSE THE LIGHT PROBES IN THE SCENE MIGHT BE NOT INITIALIZED
        NextState();

        if(testingLight)
            InvokeRepeating("NextState", 2, 4);
        else
            StartCoroutine(ApplyLightState());
    }

    private IEnumerator ApplyLightState()
    {
        // wait until the state has been randomized
        yield return new WaitUntil(() => randomized);

        // apply lighting based on the final choice
        SetState(chosenIndex);

        yield return null;
    }

    /// <summary>
    /// Iterate to the next state. Used for sequencing between areas and testing
    /// </summary>
    public void NextState()
    {
        index++;
        if (index >= lightStates.Length) index = 0;

        UpdateToState();
    }

    /// <summary>
    /// Set the state directly
    /// </summary>
    /// <param name="i">Index to use</param>
    public void SetState(int i)
    {
        if (i < 0 || i >= lightStates.Length)
        {
            index = Mathf.Clamp(i, 0, lightStates.Length);
            Debug.LogError("Requested state is out of bounds. Using closest instead");
        }
        else
        {
            index = i;
        }

        UpdateToState();
    }

    /// <summary>
    /// Based on the current state, actually apply the new lighting changes 
    /// </summary>
    private void UpdateToState()
    {
        //if (currentEnvironment != null) currentEnvironment.SetActive(false);
        
        currentState = lightStates[index];
        currentState.UpdateLightMapsSettings();
    }
}
