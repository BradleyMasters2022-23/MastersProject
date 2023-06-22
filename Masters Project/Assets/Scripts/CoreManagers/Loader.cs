using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loader : MonoBehaviour
{
    public static Loader instance;
    public static bool loading;
    private GameControls c;

    [SerializeField] private GameObject mainFrame;
    [Tooltip("Channel called when loading begins")]
    [SerializeField] private ChannelVoid onSceneChange;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            mainFrame.SetActive(false);
            DontDestroyOnLoad(gameObject);
        } 
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public Coroutine LoadToScene(string name, bool enableControls=true)
    {
        // dont load to a scene already loaded
        // if (name == SceneManager.GetActiveScene().name) return null;

        return StartCoroutine(LoadSceneAsync(name, enableControls));
    }

    private IEnumerator LoadSceneAsync(string name, bool enableControls)
    {
        loading = true;
        onSceneChange?.RaiseEvent();

        c = GameManager.controls;

        if (c != null)
        {
            c.Disable();
        }

        mainFrame.SetActive(true);

        AsyncOperation op = SceneManager.LoadSceneAsync(name, LoadSceneMode.Single);

        while (!op.isDone)
        {
            yield return null;
        }

        mainFrame.SetActive(false);

        if (c != null && enableControls)
        {
            c.PlayerGameplay.Enable();
        }
        Time.timeScale = 1f;

        loading = false;
    }
}
