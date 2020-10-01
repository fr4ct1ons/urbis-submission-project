using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Auxiliary class used to hold methods used in UnityEvents and initializing the pause menu.
/// </summary>
public class PauseMenuAuxiliar : MonoBehaviour
{
    [SerializeField] private bool createInputs = true;
    [SerializeField] private bool stopTimeOnPause = true;
    
    [Tooltip("InputActions used for enabling/disabling the menu with the esc key.")]
    [SerializeField] private PauseInputs inputs;
    [Tooltip("GameObject containing menu buttons, sliders etc.")]
    [SerializeField] private GameObject pauseMenu;
    [Tooltip("Dropdown used for loading the available resolutions.")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [Tooltip("Slider used for controlling the volume.")]
    [SerializeField] private Slider volumeSlider;
    [Tooltip("Toggle button used for setting if the game is fullscreen or not.")]
    [SerializeField] private Toggle fullscreenToggle;
    
    [Space]
    
    //[SerializeField] private AudioSource[] audioSources;

    private List<Resolution> availableResolutions;
    private bool mouseIsHidden;
    
    public delegate void PauseEvent();

    public static event PauseEvent OnGamePause, OnGameResume;

    private void Awake()
    {
        if (createInputs)
        {
            inputs = new PauseInputs();
            inputs.Pause.PauseGame.performed += PauseGameEscKey;
        }

        if (!PlayerPrefs.HasKey("IsPlayerPrefsCreated"))
        {
            CreatePlayerPrefs();
        }
    }

    private IEnumerator Start()
    {
        yield return null;
        
        SetupMenu();
    }
    
    private void CreatePlayerPrefs()
    {
        PlayerPrefs.SetInt("IsPlayerPrefsCreated", 1);
        
        PlayerPrefs.SetInt("Fullscreen", 1);
        PlayerPrefs.SetFloat("GameVolume", 1);
    
        PlayerPrefs.SetInt("ScreenResolution_width", Screen.currentResolution.width);        
        PlayerPrefs.SetInt("ScreenResolution_height", Screen.currentResolution.height);        
        PlayerPrefs.SetInt("ScreenResolution_refreshRate", Screen.currentResolution.refreshRate);
        
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Gets the player preferences values and initializes the pause menu.
    /// </summary>
    private void SetupMenu()
    {
        
        availableResolutions = new List<Resolution>(Screen.resolutions);

        List<string> possibleOptions = new List<string>();
        int currentResolutionIndex = 0;
        
        for (int i = 0; i < availableResolutions.Count; i++)
        {
            possibleOptions.Insert(i, availableResolutions[i].width + "x" + availableResolutions[i].height + "@" + availableResolutions[0].refreshRate + "Hz");
            if (availableResolutions[i].Equals(Screen.currentResolution))
            {
                currentResolutionIndex = i;
            }
        }
        
        resolutionDropdown.AddOptions(possibleOptions);
        
        //resolutionDropdown.SetValueWithoutNotify(currentResolutionIndex);

        int screenWidth;
        int screenHeight;
        int screenRefreshRate;

        fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen") == 1 ? true : false;
        volumeSlider.value = PlayerPrefs.GetFloat("GameVolume");
    
        screenWidth = PlayerPrefs.GetInt("ScreenResolution_width");        
        screenHeight = PlayerPrefs.GetInt("ScreenResolution_height");        
        screenRefreshRate = PlayerPrefs.GetInt("ScreenResolution_refreshRate");
    
        for (int i = 0; i < availableResolutions.Count; i++)
        {
            Resolution resolution = availableResolutions[i];
            if (resolution.width == screenWidth &&
                resolution.height == screenHeight &&
                resolution.refreshRate == screenRefreshRate)
            {
                resolutionDropdown.SetValueWithoutNotify(i);
            }
        }
    }

    /// <summary>
    /// Used to pause the game whenever the esc key is paused.
    /// </summary>
    /// <param name="obj"></param>
    private void PauseGameEscKey(InputAction.CallbackContext obj)
    {
        //audioSources = FindObjectsOfType<AudioSource>();

        mouseIsHidden = Cursor.visible;
        if (pauseMenu.activeSelf)
        {
            UnpauseGame();
        }
        else
        {
            PauseGame();
        }
    }

    /// <summary>
    /// Unpauses the game.
    /// </summary>
    public void UnpauseGame()
    {
        /*foreach (AudioSource source in audioSources)
            {
                source.Play();
            }*/
        if (stopTimeOnPause)
        {
            Time.timeScale = 1.0f;
        }

        pauseMenu.SetActive(false);
        Cursor.visible = mouseIsHidden;
        OnGameResume?.Invoke();
    }

    /// <summary>
    /// Pauses the game.
    /// </summary>
    public void PauseGame()
    {
        /*foreach (AudioSource source in audioSources)
        {
            source.Pause();
        }*/
        if (stopTimeOnPause)
        {
            Time.timeScale = 0.0f;
        }
        
        mouseIsHidden = Cursor.visible;
        
        pauseMenu.SetActive(true);
        OnGamePause?.Invoke();
    }

    /// <summary>
    /// Sets the game's volume and stores the value in the player preferences.
    /// </summary>
    /// <param name="val">New volume.</param>
    public void SetVolume(float val)
    {
        PlayerPrefs.SetFloat("GameVolume", val);
        AudioListener.volume = val;
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Changes the game to fullscreen or not, based on the passed value, and stores the change in the player preferences.
    /// </summary>
    /// <param name="value">Whether the game is now fullscreen or not.</param>
    public void SetFullscreen(bool value)
    {
        PlayerPrefs.SetInt("Fullscreen", value ? 1 : 0); // 1 means true, 0 means false.
        Screen.fullScreen = value;
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Closes the game.
    /// </summary>
    public void CloseGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Sets the game's resolution, with the availableResolutions array.
    /// </summary>
    /// <param name="index">Index of the available resolution to be set.</param>
    public void SetResolution(int index)
    {
        PlayerPrefs.SetInt("ScreenResolution_width", availableResolutions[index].width);
        PlayerPrefs.SetInt("ScreenResolution_height", availableResolutions[index].height);
        PlayerPrefs.SetInt("ScreenResolution_refreshRate", availableResolutions[index].refreshRate);
        
        Screen.SetResolution(availableResolutions[index].width, availableResolutions[index].height, 
            Screen.fullScreenMode, availableResolutions[index].refreshRate);
        
        PlayerPrefs.Save();
    }
    
    private void OnEnable()
    {
        inputs?.Enable();
    }

    private void OnDisable()
    {
        inputs?.Disable();
    }
}