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

        sfxSource.volume = sfxSlider.value;
    }

    void Update()
    {
        if (togglePauseMenu.WasPerformedThisFrame())
        {
            TogglePauseMenu();
        }

        sfxSlider.onValueChanged.AddListener(delegate { sfxSource.volume = sfxSlider.value; PlayerPrefs.SetFloat("SFX_Volume", sfxSlider.value); });
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
