using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuAuxiliary : MonoBehaviour
{
    [SerializeField] private GameObject cityManager;
    [SerializeField] private GameObject gameplayUI;
    [SerializeField] private CustomSceneAsset gameScene;
    [SerializeField] private Button continueButton;
    [SerializeField] private CameraMovement cameraMovement;
    [SerializeField] private CinemachineVirtualCameraBase gameplayCamera, mainMenuCamera;
    [SerializeField] private float mainMenuRotationSpeed;

    private bool hasSaveData = true;

    private CinemachineFreeLook mainMenuFreelook;
    void Start()
    {
        cityManager.SetActive(false);
        Time.timeScale = 1.0f;
        mainMenuFreelook = (CinemachineFreeLook) mainMenuCamera;
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

    public void NewGame()
    {
        if (!hasSaveData)
        {
            cityManager.SetActive(true);
            gameplayUI.SetActive(true);
            gameObject.SetActive(false);
            cameraMovement.enabled = true;

            gameplayCamera.Priority = 10;
            mainMenuCamera.Priority = 0;
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

        gameplayCamera.Priority = 10;
        mainMenuCamera.Priority = 0;
    }
}
