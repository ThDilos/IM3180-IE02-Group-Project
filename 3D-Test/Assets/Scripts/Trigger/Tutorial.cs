using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [Tooltip("Currently only support showing 1 icon")]
    public Texture icon; // The Icon to show
    public List<string> lines; // The Line to display

    [Header("Overflow Action")]
    [Tooltip("Dialog Beyond this length will be followed by a '-' and continued on the next page.")]
    [SerializeField] private int selfWarpLength = 130;

    // Runtime Vars
    GameObject dialogBox;
    DialogBox dialogScript;
    bool triggerOnStartUp = true;

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
        dialogScript.SetLines(lines);
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
