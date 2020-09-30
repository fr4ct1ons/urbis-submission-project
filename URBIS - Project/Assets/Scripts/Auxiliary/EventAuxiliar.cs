using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Collection of simple events that can be called on UnityEvents.
/// </summary>
public class EventAuxiliar : MonoBehaviour
{
    private void Awake()
    {
        if (TryGetComponent<Button>(out Button button))
        {
            //button.OnPointerDown();
        }
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadSceneAsset(CustomSceneAsset asset)
    {
        SceneManager.LoadScene(asset.SceneName);
    }

    public void SetTimeScale(float value)
    {
        Time.timeScale = value;
    }

    public void CloseGame()
    {
        Application.Quit();
    }

    public void PrintValue(string value)
    {
        Debug.Log(value);
    }
}
