using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    [Tooltip("Currently only support showing 1 icon")]
    public Texture icon = null; // The Icon to show
    public List<string> lines; // The Line to display

    [Header("Overflow Action")]
    [Tooltip("Dialog Beyond this length will be followed by a '-' and continued on the next page.")]
    [SerializeField] private int selfWarpLength = 130;

    [Header("Transition to scene after dialog")]
    [SerializeField] private bool activated = false;
    [SerializeField] private int sceneIndex = -1;
   

    // Runtime Vars
    GameObject dialogBox;
    DialogBox dialogScript;
    bool triggerOnStartUp = true;

    private bool triggered = false;

    private void Start()
    {
        dialogScript = DialogBox.Instance;
        dialogBox = dialogScript.gameObject;

        OverflowDetection();

        if (triggerOnStartUp) Trigger();
    }

    public void Trigger()
    {
        dialogBox.SetActive(true);

        dialogScript.iconImage.gameObject.SetActive(true);
        dialogScript.SetLines(lines, icon);

        triggered = true;
    }

    private void FixedUpdate()
    {
        if (!dialogBox.gameObject.activeSelf && activated && triggered)
        {
            if (sceneIndex != -1)
                SceneManager.LoadScene(sceneIndex);
            else
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    private void OverflowDetection()
    {
        if (lines.Count == 0) return;

        for (int i = 0; i < lines.Count; i++)
        {
            string line = lines[i];
            if (line.Length > selfWarpLength)
            {
                string cutted = line.Substring(selfWarpLength); // Gets the remaining part
                line = line.Substring(0, selfWarpLength) + "-";

                lines[i] = line; // Update the current line
                lines.Insert(i + 1, cutted);
            }
        }
    }
}
