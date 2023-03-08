using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public struct ResOption
{
    public ResOption(int w, int h)
    {
        width= w;
        height= h;
    }
    public ResOption(string data)
    {
        // Resolution storedn as a [WIDTH]x[HEIGHT] string, 
        // so split into substring and parse
        string[] temp = data.Split('x');
        width = int.Parse(temp[0]);
        height = int.Parse(temp[1]);
    }

    public int width;
    public int height;

    public override string ToString()
    {
        return (width + "x" + height);
    }
}

[System.Serializable]
public struct RefreshOption
{
    public RefreshOption(int r)
    {
        refreshRate= r;
    }

    public int refreshRate;

    public override string ToString()
    {
        return refreshRate+" Hz";
    }
}

[System.Serializable]
public class FullResolutionData
{
    public int width;
    public int height;
    public int refreshRate;

    public FullResolutionData(ResOption res, RefreshOption refresh)
    {
        width = res.width;
        height = res.height;
        refreshRate= refresh.refreshRate;
    }
}

public enum WindowedMode
{
    Fullscreen, 
    BorderlessWindow,
    Window
}

public class GraphicSettings : MonoBehaviour
{
    #region Resolutions

    [SerializeField] private GameObject confirmResolutionBox;

    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] TMP_Dropdown refreshRateDropdown;
    [SerializeField] TMP_Dropdown windowModeDropdown;

    private ResOption currentResolution;
    private ResOption lastResolution;
    private RefreshOption currentRefreshRate;

    private List<ResOption> resolutions = new List<ResOption>();
    private List<RefreshOption> refreshOptions = new List<RefreshOption>();

    private WindowedMode currentWindowMode;

    private Coroutine resetRoutine;

    private IEnumerator Start()
    {
        PlayerPrefs.DeleteKey("DefaultResolution");
        PlayerPrefs.DeleteKey("DefaultRefreshRate");

        resolutions = new List<ResOption>();
        refreshOptions = new List<RefreshOption>();

        yield return GetPotentialResolutions();

        PopulateDropdown();

        yield return null;
    }

    /// <summary>
    /// Get all potential resolutions and refresh rates that can be used.
    /// </summary>
    /// <returns></returns>
    private IEnumerator GetPotentialResolutions()
    {
        // get and load current data
        Resolution currRes = Screen.currentResolution;
        currentResolution = new ResOption(currRes.width, currRes.height);
        Debug.Log("Current resolution grabbed is : " + currentResolution.ToString());
        currentRefreshRate = new RefreshOption(currRes.refreshRate);

        // Save DEFAULTS on the first time
        if(!PlayerPrefs.HasKey("DefaultResolution"))
        {
            PlayerPrefs.SetString("DefaultResolution", currentResolution.ToString());
        }
        if (!PlayerPrefs.HasKey("DefaultRefreshRate"))
        {
            Debug.Log("Saved refresh rate: " + currRes.refreshRate);
            PlayerPrefs.SetInt("DefaultRefreshRate", currRes.refreshRate);
        }

        currentWindowMode = ConvertMode(Screen.fullScreenMode);

        // Get all potential resolutions and refresh rates
        // Divide these two options so they can be in their own dropdowns
        foreach (Resolution r in Screen.resolutions)
        {
            ResOption res = new ResOption(r.width, r.height);
            RefreshOption refresh = new RefreshOption(r.refreshRate);
            if (!resolutions.Contains(res))
                resolutions.Add(res);
            if (!refreshOptions.Contains(refresh))
                refreshOptions.Add(refresh);
        }

        yield return null;
    }

    /// <summary>
    /// Populate all dropdowns with the appropriate data
    /// </summary>
    private void PopulateDropdown()
    {
        // Get all resolutions, track the currently selected one, setup dropdown
        int selectedResIndex = 0;
        List<string> printResolutions = new List<string>();
        for (int i = 0; i < resolutions.Count; i++)
        {
            // If the passed in resolution is the current, track its index for later
            if (resolutions[i].Equals(currentResolution))
                selectedResIndex = i;

            // Add all options in a string format to the options list
            printResolutions.Add(resolutions[i].ToString());
        }
        // Clear old list and apply the new option list
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(printResolutions);
        // Use previous index to set the current value to the current resolution index
        resolutionDropdown.SetValueWithoutNotify(selectedResIndex);


        // Pull saved refresh rate
        int savedRefreshRate = PlayerPrefs.GetInt("RefreshRate", 0);
        if (savedRefreshRate == 0)
            savedRefreshRate = Screen.currentResolution.refreshRate;
        currentRefreshRate = new RefreshOption(savedRefreshRate);

        // Get all refresh rates, track the currently selected one, setup dropdown
        int selectRefreshIndex = 0;
        List<string> printRefreshs = new List<string>();
        for (int i = 0; i < refreshOptions.Count; i++)
        {
            // If the passed in refreshrate is the current, track its index for later
            if (refreshOptions[i].Equals(currentRefreshRate))
                selectRefreshIndex = i;

            // Add all options in a string format to the options list
            printRefreshs.Add(refreshOptions[i].ToString());
        }
        // Clear old list and apply the new option list
        refreshRateDropdown.ClearOptions();
        refreshRateDropdown.AddOptions(printRefreshs);
        // Use previous index to set the current value to the current refreshrate index
        refreshRateDropdown.SetValueWithoutNotify(selectRefreshIndex);


        // Update dropdown for fullscreen
        windowModeDropdown.SetValueWithoutNotify((int)currentWindowMode);
    }

    public void ApplyResolution()
    {
        lastResolution = currentResolution;
        currentResolution = resolutions[resolutionDropdown.value];
        Screen.SetResolution(currentResolution.width, currentResolution.height, GetWindowMode(), currentRefreshRate.refreshRate);

        resetRoutine = StartCoroutine(WaitToRevert());
    }

    public void ApplyWindowMode()
    {
        // Do this conversion as the normal fullscreen mode includes
        // MAC only options, which we do not support
        currentWindowMode = (WindowedMode) windowModeDropdown.value;


        // apply
        Screen.SetResolution(currentResolution.width, currentResolution.height, GetWindowMode(), currentRefreshRate.refreshRate);
    }

    private FullScreenMode GetWindowMode()
    {
        FullScreenMode mode = FullScreenMode.ExclusiveFullScreen;
        switch (currentWindowMode)
        {
            case WindowedMode.Fullscreen:
                {
                    mode = FullScreenMode.ExclusiveFullScreen;
                    break;
                }
            case WindowedMode.BorderlessWindow:
                {
                    mode = FullScreenMode.FullScreenWindow;
                    break;
                }
            case WindowedMode.Window:
                {
                    mode = FullScreenMode.Windowed;
                    break;
                }
            default:
                {
                    mode = FullScreenMode.ExclusiveFullScreen;
                    break;
                }
        }
        return mode;
    }
    private WindowedMode ConvertMode(FullScreenMode fsMode)
    {
        WindowedMode mode;

        switch (fsMode)
        {
            case FullScreenMode.ExclusiveFullScreen:
                {
                    mode = WindowedMode.Fullscreen;
                    break;
                }
            case FullScreenMode.FullScreenWindow:
                {
                    mode = WindowedMode.BorderlessWindow;
                    break;
                }
            case FullScreenMode.Windowed:
                {
                    mode = WindowedMode.Window;
                    break;
                }
            default:
                {
                    mode = WindowedMode.Fullscreen;
                    break;
                }
        }
        Debug.Log($"Convert mode input {fsMode} | output {mode}");

        return mode;
    }
    private IEnumerator WaitToRevert()
    {
        confirmResolutionBox.SetActive(true);

        GameManager.instance.TempLockUIFlow(true);

        // Wait a few seconds. This can be broken from outside this
        yield return new WaitForSecondsRealtime(10f);

        GameManager.instance.TempLockUIFlow(false);

        RevertResolution();
    }

    public void ConfirmResolution()
    {
        StopCoroutine(resetRoutine);
        confirmResolutionBox.SetActive(false);
        GameManager.instance.TempLockUIFlow(false);

        // No need to save, Unity handles that automatically
    }

    public void RevertResolution()
    {
        if(resetRoutine != null)
        {
            StopCoroutine(resetRoutine);
            resetRoutine = null;
        }

        confirmResolutionBox.SetActive(false);

        // revert the resolution to previous
        currentResolution = lastResolution;
        lastResolution = default;
        Screen.SetResolution(currentResolution.width, currentResolution.height, GetWindowMode(), currentRefreshRate.refreshRate);

        // update the selected choice in the dropdown
        for (int i = 0; i < resolutionDropdown.options.Count; i++)
        {
            if (currentResolution.ToString().Equals(resolutionDropdown.options[i].text))
            {
                resolutionDropdown.SetValueWithoutNotify(i);
                break;
            }
        }
    }

    public void ApplyRefreshRate()
    {
        currentRefreshRate = refreshOptions[refreshRateDropdown.value];
        Screen.SetResolution(currentResolution.width, currentResolution.height, GetWindowMode(), currentRefreshRate.refreshRate);

        PlayerPrefs.SetInt("RefreshRate", currentRefreshRate.refreshRate);
    }

    private void OnApplicationQuit()
    {
        // Quickly revert resolution to previous option on disable incase the player crashes, so it doesnt revert to it before
        if (resetRoutine != null)
        {
            RevertResolution();
        }
    }

    private void OnDisable()
    {
        // Quickly revert resolution to previous option on disable incase the player crashes, so it doesnt revert to it before
        if (resetRoutine != null)
        {
            RevertResolution();
        }
    }

    public void RevertToDefault()
    {
        // Load in defaults
        ResOption defaultResolution = new ResOption(PlayerPrefs.GetString("DefaultResolution"));
        int defaultRefresh = PlayerPrefs.GetInt("DefaultRefreshRate");

        currentResolution = defaultResolution;
        currentRefreshRate = new RefreshOption(defaultRefresh);
        windowModeDropdown.value = 0;

        // delete old saved
        PlayerPrefs.DeleteKey("RefreshRate");

        PopulateDropdown();
        //Screen.SetResolution(currentResolution.width, currentResolution.height, FullScreenMode.ExclusiveFullScreen, currentRefreshRate.refreshRate);
        
    }

    #endregion
}

