using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogBox : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public RawImage iconImage;
    public float textSpeed;
    [SerializeField] private InputActionAsset inputActions;

    // Runtime Vars
    private InputAction interactAction;
    private int index;
    private List<string> lines;

    // Start is called before the first frame update
    void Awake()
    {
        gameObject.SetActive(false); // Hide when Start
        textComponent.text = string.Empty; // Hide Text

        InputActionMap map = inputActions.FindActionMap("Utils");
        interactAction = map.FindAction("Interact");
    }

    // Update is called once per frame
    void Update()
    {
        if (interactAction.WasPerformedThisFrame())
        {
            if (textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index];
            }
        }
    }

    public void SetLines(List<string> lines)
    {
        StopAllCoroutines(); // Prevent lines interfering with each otherrs
        this.lines = lines;
        this.index = -1;
        NextLine();
        SetIcon(null);
    }

    public void SetLines(List<string> lines, Texture icon)
    {
        SetLines(lines);
        SetIcon(icon);
    }

    private void SetIcon(Texture icon)
    {
        iconImage.gameObject.SetActive(true);
        if (icon != null)
        {
            iconImage.texture = icon;
        }
        else
        {
            Texture placeholder;
            SwitchCharacter sc = GameObject.Find("Player").GetComponent<SwitchCharacter>();
            DialogPopUp dpu = GameObject.Find("Player").GetComponent<DialogPopUp>();
            switch (sc.activatedCharacter)
            {
                case SwitchCharacter.ActivatedCharacter.CAT:
                    placeholder = dpu.catIcon;
                    break;
                case SwitchCharacter.ActivatedCharacter.GOOSE:
                    placeholder = dpu.gooseIcon;
                    break;
                case SwitchCharacter.ActivatedCharacter.BEAR:
                    placeholder = dpu.bearIcon;
                    break;
                default:
                    placeholder = null;
                    break;
            }
            iconImage.texture = placeholder;
        }
    }

    System.Collections.IEnumerator TypeLine()
    {
        // Type each character 1 by 1
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(1 / textSpeed);
        }
    }

    void NextLine()
    {
        if (index < lines.Count - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

}
