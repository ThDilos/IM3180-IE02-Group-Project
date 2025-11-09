using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI; 

public class labreport : MonoBehaviour
{
    public GameObject abstractPanel;
    public GameObject charPanel;
    public GameObject lorePanel;
    public GameObject newPanel;
    public Button openreportButton;
    public Button abstractButton;
    public Button charButton;
    public Button loreButton;
    public Button newButton;

    public AudioSource audioSource;
    public AudioClip flipPage;
    public AudioClip closeBoard;

    // Runtime Vars
    private InputAction labReportAction;
    private bool panelOpen = false;
    private bool isOpening = false;

    void Start()
    {
        if (!GameController.Instance.gotLabReport)
        {
            gameObject.SetActive(true);
            openreportButton.gameObject.SetActive(false);
        }

        // Ensure the pause panel is not active at the start
        abstractPanel.SetActive(false);
        charPanel.SetActive(false);
        lorePanel.SetActive(false);
        newPanel.SetActive(false);

        abstractButton.onClick.AddListener(Abstract);
        charButton.onClick.AddListener(Characters);
        loreButton.onClick.AddListener(Lore);
        newButton.onClick.AddListener(ComingSoon);

        openreportButton.onClick.AddListener(openReport);
        InputActionMap map = GameController.Instance.inputActions.FindActionMap("Utils");
        labReportAction = map.FindAction("LabReport");
    }

    void Update()
    {
        // Check if the F key is pressed to toggle the pause menu
        if (labReportAction.WasPerformedThisFrame() && GameController.Instance.gotLabReport)
        {
            openReport();
        }
    }
    public void openReport()
    {
        Debug.Log("OpenReportToggle clicked. panelOpen = " + panelOpen);

        if (isOpening) return; // ignore multiple triggers
        isOpening = true;

        if (panelOpen)
        {
            ResumeGame();  // Resume the game if the menu is currently open
        }
        else
        {
            Abstract();  // Pause the game if it's running
        }
        isOpening = false;
    }
    public void Abstract()
    {
        panelOpen = true;
        audioSource.PlayOneShot(flipPage);
        abstractPanel.SetActive(true);  // Show the panel
        charPanel.SetActive(false);
        lorePanel.SetActive(false);
        newPanel.SetActive(false);
        GameController.Instance.PauseGame();
    }
    public void Characters()
    {
        panelOpen = true;
        audioSource.PlayOneShot(flipPage);
        abstractPanel.SetActive(false);
        charPanel.SetActive(true);
        lorePanel.SetActive(false);
        newPanel.SetActive(false);
    }
    public void Lore()
    {
        panelOpen = true;
        audioSource.PlayOneShot(flipPage);
        abstractPanel.SetActive(false);
        charPanel.SetActive(false);
        lorePanel.SetActive(true);
        newPanel.SetActive(false);
    }
    public void ComingSoon()
    {
        panelOpen = true;
        audioSource.PlayOneShot(flipPage);
        abstractPanel.SetActive(false);
        charPanel.SetActive(false);
        lorePanel.SetActive(false);
        newPanel.SetActive(true);
    }
    public void ResumeGame()
    {
        panelOpen = false;
        audioSource.PlayOneShot(closeBoard);
        abstractPanel.SetActive(false);  // Hide the panel
        charPanel.SetActive(false);
        lorePanel.SetActive(false);
        newPanel.SetActive(false);
        GameController.Instance.ResumeGame();
    }
}
