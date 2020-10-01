using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class MainMenuAuxiliary : MonoBehaviour
{
    [SerializeField] private GameObject cityManager;
    [SerializeField] private GameObject gameplayUI;
    [SerializeField] private CameraMovement cameraMovement;

    [SerializeField] private CinemachineVirtualCameraBase gameplayCamera, mainMenuCamera;
    
    void Start()
    {
        cityManager.SetActive(false);
    }

    public void CloseGame()
    {
        Application.Quit();
    }

    public void NewGame()
    {
        cityManager.SetActive(true);
        gameplayUI.SetActive(true);
        gameObject.SetActive(false);
        cameraMovement.enabled = true;

        gameplayCamera.Priority = 10;
        mainMenuCamera.Priority = 0;
    }
}
