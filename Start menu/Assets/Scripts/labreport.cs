using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement;
using TMPro;

public class labreport : MonoBehaviour
{
    public GameObject abstractPanel;
    public GameObject charPanel;
    public GameObject lorePanel;
    public GameObject newPanel;
    public GameObject mainPanel;
    public Button openreportButton;
    public Button abstractButton;
    public Button charButton;
    public Button loreButton;
    public Button newButton;

    public AudioClip flipPage;
    public AudioClip closeBoard;

    private bool panelOpen = false;
    private bool isOpening = false;

    void Start()
    {
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
    }

    void Update()
    {
        // Check if the F key is pressed to toggle the pause menu
        if (Input.GetKeyDown(KeyCode.F))
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
        AudioSource.PlayClipAtPoint(flipPage, transform.position, 1f);
        abstractPanel.SetActive(true);  // Show the panel
        charPanel.SetActive(false);
        lorePanel.SetActive(false);
        newPanel.SetActive(false);
    }
    public void Characters()
    {
        panelOpen = true;
        AudioSource.PlayClipAtPoint(flipPage, transform.position, 1f);
        abstractPanel.SetActive(false);
        charPanel.SetActive(true);
        lorePanel.SetActive(false);
        newPanel.SetActive(false);
    }
    public void Lore()
    {
        panelOpen = true;
        AudioSource.PlayClipAtPoint(flipPage, transform.position, 1f);
        abstractPanel.SetActive(false);
        charPanel.SetActive(false);
        lorePanel.SetActive(true);
        newPanel.SetActive(false);
    }
    public void ComingSoon()
    {
        panelOpen = true;
        AudioSource.PlayClipAtPoint(flipPage, transform.position, 1f);
        abstractPanel.SetActive(false);
        charPanel.SetActive(false);
        lorePanel.SetActive(false);
        newPanel.SetActive(true);
    }
    public void ResumeGame()
    {
        panelOpen = false;
        AudioSource.PlayClipAtPoint(closeBoard, transform.position, 1f);
        abstractPanel.SetActive(false);  // Hide the panel
        charPanel.SetActive(false);
        lorePanel.SetActive(false);
        newPanel.SetActive(false);
        mainPanel.SetActive(true);
    }
}
