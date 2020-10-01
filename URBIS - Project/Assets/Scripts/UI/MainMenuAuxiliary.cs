using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuAuxiliary : MonoBehaviour
{
    [Tooltip("Object containing the CityManager, to be disabled when loading the game.")]
    [SerializeField] private GameObject cityManager;
    [Tooltip("Regular gameplay UI.")]
    [SerializeField] private GameObject gameplayUI;
    [SerializeField] private CustomSceneAsset gameScene;
    [Tooltip("Button that is disabled if there is no save data.")]
    [SerializeField] private Button continueButton;
    [Tooltip("CameMovement object, enabled when the game starts.")]
    [SerializeField] private CameraMovement cameraMovement;
    [Tooltip("Object used to control the day-night cycle.")]
    [SerializeField] private DayNightCycle dayNightCycle;

    [SerializeField] private CinemachineVirtualCameraBase gameplayCamera;
    [SerializeField] private CinemachineVirtualCameraBase mainMenuCamera;
    [SerializeField] private float mainMenuRotationSpeed;

    private bool hasSaveData = true;

    private CinemachineFreeLook mainMenuFreelook;

    void Start()
    {
        cityManager.SetActive(false);
        Time.timeScale = 1.0f;
        mainMenuFreelook = (CinemachineFreeLook) mainMenuCamera;

        dayNightCycle.enabled = false;
        
        if (!CityManager.Instance.LoadSaveData())
        {
            continueButton.interactable = false;
            hasSaveData = false;
        }
    }

    private void Update()
    {
        mainMenuFreelook.m_XAxis.Value = mainMenuRotationSpeed;
    }

    
    public void CloseGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Creates a new save data if there is no save game. Otherwise, starts the game normally.
    /// </summary>
    public void NewGame()
    {
        if (!hasSaveData)
        {
            ContinueGame();
        }
        else
        {
            SaveDataManager.TrySaveData(null, 1);
            SceneManager.LoadScene(gameScene.SceneName);
        }
    }

    public void ContinueGame()
    {
        cityManager.SetActive(true);
        gameplayUI.SetActive(true);
        gameObject.SetActive(false);
        cameraMovement.enabled = true;
        dayNightCycle.enabled = true;

        gameplayCamera.Priority = 10;
        mainMenuCamera.Priority = 0;
    }
}
