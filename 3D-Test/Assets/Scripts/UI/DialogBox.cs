using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogBox : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
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
        this.lines = lines;
        this.index = -1;
        NextLine();
    }

    void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
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
