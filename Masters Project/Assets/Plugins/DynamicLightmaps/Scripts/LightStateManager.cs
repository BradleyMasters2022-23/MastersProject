using DynamicLightmaps;
using UnityEngine;
using Sirenix.OdinInspector;

public class LightStateManager : MonoBehaviour
{
    [SerializeField] private GameObject[] environments;
    [SerializeField] private LightState[] lightStates;

    [SerializeField, ReadOnly] private int index = -1;
    [SerializeField] private bool testing;

    private LightState currentState;
    private GameObject currentEnvironment;

    private void Awake()
    {
        if (environments.Length != lightStates.Length)
            throw new System.Exception("Environment and Light States Mismatch");
    }

    private void Start()
    {
        // NOTE IF YOU WANT TO CHANGE THE STATE OF THE LIGHTMAPS AT THE BEGINING OF THE GAME
        // CHANGE IT IN START NOT AWAKE BECAUSE THE LIGHT PROBES IN THE SCENE MIGHT BE NOT INITIALIZED
        NextState();

        if(testing)
            InvokeRepeating("NextState", 2, 4);
    }

    /// <summary>
    /// Iterate to the next state. Used for sequencing between areas and testing
    /// </summary>
    public void NextState()
    {
        index++;
        if (index >= environments.Length) index = 0;

        UpdateToState();
    }

    /// <summary>
    /// Set the state directly
    /// </summary>
    /// <param name="i">Index to use</param>
    public void SetState(int i)
    {
        if (i < 0 || i >= environments.Length)
        {
            index = Mathf.Clamp(i, 0, environments.Length);
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
        if (currentEnvironment != null) currentEnvironment.SetActive(false);
        currentState = lightStates[index];
        currentEnvironment = environments[index];

        currentEnvironment.SetActive(true);
        currentState.UpdateLightMapsSettings();
    }
}
