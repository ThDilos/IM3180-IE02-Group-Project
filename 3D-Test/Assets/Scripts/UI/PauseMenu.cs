using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private AudioSource bgmSource;

    // Runtime Vars
    private GameController gc;
    private InputActionAsset inputActions;
    private InputAction togglePauseMenu;
    private bool gamePaused = false;

    private void Start()
    {
        gc = GameController.Instance;
        inputActions = gc.inputActions;
        InputActionMap map = inputActions.FindActionMap("Utils");
        togglePauseMenu = map.FindAction("Pause");
        settingPanel.SetActive(gamePaused);

        if (sfxSlider != null && sfxSource != null)
            sfxSource.volume = sfxSlider.value;
        if (bgmSlider != null && bgmSource != null)
            bgmSource.volume = bgmSlider.value;
    }

    void Update()
    {
        if (togglePauseMenu.WasPerformedThisFrame())
        {
            TogglePauseMenu();
        }
        if (sfxSlider != null && sfxSource != null)
            sfxSlider.onValueChanged.AddListener(delegate { sfxSource.volume = sfxSlider.value; PlayerPrefs.SetFloat("SFX_Volume", sfxSlider.value); });
        if (bgmSlider != null && bgmSource != null) 
            bgmSlider.onValueChanged.AddListener(delegate { bgmSource.volume = bgmSlider.value; PlayerPrefs.SetFloat("BGM_Volume", bgmSlider.value); });
    }

    public void Quitgame()
    {
        Debug.Log("Quitted Game");
        Application.Quit();
    }

    public void RestartGame()
    {
        // Reload Current Scene
        gc.ResumeGame();
        if (SceneManager.GetActiveScene().buildIndex == gc.labSceneIndex)
            gc.timesInLab--;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void TogglePauseMenu()
    {
        gamePaused = !gamePaused;
        settingPanel.SetActive(gamePaused);

        if (gamePaused)
        {
            gc.PauseGame();
        }
        else
        {
            gc.ResumeGame();
        }
    }
}
