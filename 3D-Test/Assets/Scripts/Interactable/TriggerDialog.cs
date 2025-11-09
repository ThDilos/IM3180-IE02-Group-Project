using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDialog : MonoBehaviour, IInteractable
{
    [Tooltip("Currently only support showing 1 icon")]
    public Texture icon; // The Icon to show
    public List<string> lines; // The Line to display

    [Header("Set an Animation Param to True Upon Destroyed?")]
    public bool destroyAfterInteraction = false;
    [SerializeField] private Animator animator;
    [SerializeField] private string animationParam;

    [Header("Only who can pickup")]
    [SerializeField] private bool charLimitEnabled = false;
    [SerializeField] private SwitchCharacter.ActivatedCharacter characterLimit;

    [Header("Progressive Lines - Repeat the following lines when 1st lines is already triggered")]
    [Tooltip("Whether the 1st sequence of lines only show once, and the next sequence of lines are repeated afterwards.")] public bool progressiveLine;
    public List<string> repeatedLines; // The Lines that are repeated
    public bool interacting = false;
    private bool alreadyInteracted = false;

    [Header("Overflow Action")]
    [Tooltip("Dialog Beyond this length will be followed by a '-' and continued on the next page.")]
    [SerializeField] private int selfWarpLength = 130;

    [Header("Self Close Dialog")]
    [Tooltip("Whether this dialog will self-close when player is far away")]
    [SerializeField] private bool autoClose = true;
    [SerializeField] private float autoCloseDistance = 5.0f;
    [Tooltip("Used for Progressive Lines. If True, player may permanently miss the cancelled parts.")]
    [SerializeField] private bool autoCloseMeansCompletedTheWholeDialog = false;

    [Header("Floating Text")]
    [SerializeField] private string alternativeFloatingText = null;
    [SerializeField] private float yOffsetUp = 0.5f;

    // Runtime Vars
    GameObject dialogBox;
    DialogBox dialogScript;
    Transform playerTransform; // Used to self-close when player get far

    private void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
        // Get DialogBox instance :3
        dialogScript = DialogBox.Instance;
        dialogBox = dialogScript.gameObject;

        OverflowDetection();
    }

    private void Update()
    {
        if (interacting)
        {
            interacting = dialogBox.activeSelf;
            // Self Close when player get Far
            if (autoClose && Vector3.Distance(playerTransform.position, transform.position) > autoCloseDistance)
                ResetDialog();
        }
    }

    public void Interact()
    {
        dialogBox.SetActive(true);
        interacting = true;

        dialogScript.iconImage.gameObject.SetActive(true);

        if (!alreadyInteracted || repeatedLines.Count == 0) dialogScript.SetLines(lines);
        else dialogScript.SetLines(repeatedLines, icon);

        alreadyInteracted = progressiveLine;
        if (destroyAfterInteraction)
        {
            StartCoroutine("DestroyObjectWithBear");
        }
    }

    private void ResetDialog()
    {
        dialogBox.SetActive(false);
        alreadyInteracted = autoCloseMeansCompletedTheWholeDialog;
    }

    public Vector3 getPosition() 
    {
        return new Vector3(transform.position.x, transform.position.y + yOffsetUp, transform.position.z);
    }

    public bool Condition()
    {
        Debug.Log(!dialogBox.activeSelf + " and (" + !charLimitEnabled + " or " + (charLimitEnabled && GameObject.Find("Player").GetComponent<SwitchCharacter>().activatedCharacter == characterLimit) + ")");
            return !dialogBox.activeSelf && (!charLimitEnabled ||
            (charLimitEnabled && GameObject.Find("Player").GetComponent<SwitchCharacter>().activatedCharacter == characterLimit));
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

    public string AlternativeText()
    {
        return alternativeFloatingText;
    }

    IEnumerator DestroyObjectWithBear()
    {
        if (GameObject.Find("Player").GetComponent<SwitchCharacter>().activatedCharacter == SwitchCharacter.ActivatedCharacter.BEAR)
        {
            GameObject.Find("Player").GetComponent<Animator>().SetTrigger("UsingAbility");
        }
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (animationParam.Length > 0 && animator != null)
        {
            animator.SetBool(animationParam, true);
        }
    }
}
