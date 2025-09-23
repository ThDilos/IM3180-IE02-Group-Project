
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SwitchCharacter))]
public class DialogPopUp : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [Header("The Icons displayed on the dialog box")]
    public Texture bearIcon;
    public Texture gooseIcon;
    public Texture catIcon;

    [SerializeField] private InputActionAsset inputs;
    public enum CommonDialog
    {
        WaterWarning,
        Silly
    }

    [System.Serializable] public struct DialogLines
    {
        [Header("If there are more than 1 elements, the line will be picked randomly :3")]
        public List<string> catLines;
        public List<string> bearLines;
        public List<string> gooseLines;

        public List<string> GetLines(SwitchCharacter.ActivatedCharacter activatedCharacter)
        {
            switch (activatedCharacter)
            {
                case SwitchCharacter.ActivatedCharacter.GOOSE:
                    return gooseLines;
                case SwitchCharacter.ActivatedCharacter.BEAR:
                    return bearLines;
                case SwitchCharacter.ActivatedCharacter.CAT:
                    return catLines;
                default:
                    return null;
            }
        }
    }

    [SerializeField] private DialogLines waterWarning;
    [SerializeField] private DialogLines silly;

    // Runtime Vars
    private DialogBox dialogScript;
    private InputAction test;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InputActionMap maps = inputs.FindActionMap("Utils");
        test = maps.FindAction("Test");
        dialogScript = dialogBox.GetComponent<DialogBox>();
        if (dialogBox == null)
        {
            Debug.LogError("Dialog Box on DialogPopUp on Player is Empty!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (test.WasPerformedThisFrame())
        {
            PopUpDialog(CommonDialog.Silly);
        }
    }

    void PopUpDialog(List<string> lines)
    {
        // Choose Random if more than 1 element
        List<string> temp = new List<string>();
        if (lines.Count > 1)
        {
            temp.Add(lines[Random.Range(0, lines.Count)]);
        }
        else temp = lines;

        dialogBox.SetActive(true);
        dialogScript.SetLines(temp);
    }

    void PopUpDialog(CommonDialog option)
    {
        SwitchCharacter sc = transform.GetComponent<SwitchCharacter>();
        switch (option)
        {
            case CommonDialog.WaterWarning:
                PopUpDialog(waterWarning.GetLines(sc.activatedCharacter));
                break;
            case CommonDialog.Silly:
                PopUpDialog(silly.GetLines(sc.activatedCharacter));
                break;
        }
    }
}
