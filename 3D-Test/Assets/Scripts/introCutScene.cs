using UnityEngine;
using UnityEngine.SceneManagement;

public class introCutScene : MonoBehaviour
{
    public GameObject[] panels;  // Assign all panels in Inspector
    private int currentIndex = 0;

    void Start()
    {
        // Hide all panels first
        foreach (var panel in panels)
            panel.SetActive(false);

        // Show the first panel
        if (panels.Length > 0)
            panels[0].SetActive(true);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // left click
        {
            NextPanel();
        }
    }

    void NextPanel()
    {
        currentIndex++;

        if (currentIndex<panels.Length)
        {
            // Show next panel
            panels[currentIndex].SetActive(true);
        }
        else
        {
            // End of cutscene
            Debug.Log("Cutscene finished!");
            SceneManager.LoadScene("Level0_Lab");
        }
    }
}
