using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loader : MonoBehaviour
{
    public static Loader instance;

    private GameControls c;

    [SerializeField] private GameObject mainFrame;

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

    public void LoadToScene(string name)
    {
        // dont load to a scene already loaded
        if (name == SceneManager.GetActiveScene().name) return;

        StartCoroutine(LoadSceneAsync(name));
    }

    private IEnumerator LoadSceneAsync(string name)
    {
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

        if (c != null)
        {
            c.Enable();
        }
    }
}
