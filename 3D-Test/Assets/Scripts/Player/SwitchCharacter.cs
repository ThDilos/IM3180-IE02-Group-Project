using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements.Experimental;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]

public class SwitchCharacter : MonoBehaviour
{

    public enum ActivatedCharacter 
    {
        GOOSE,
        BEAR,
        CAT
    }

    [Tooltip("Which Character the player will start as?")]
    [SerializeField] public ActivatedCharacter activatedCharacter;

    [Header("Put in the characters, STRICTLY in the order of Goose, Bear, Cat")]
    [SerializeField] GameObject[] characters;
    [SerializeField] float bearColliderHeightMultiplier = 1.8f;
    public InputActionAsset inputActions;

    private InputAction char1;
    private InputAction char2;
    private InputAction char3;

    private BoxCollider bc;
    private Vector3 originalBCSize;
    private Vector3 originalBCCenter;

    private Movement movementScript;

    private void Awake()
    {
        bc = gameObject.GetComponent<BoxCollider>();
        originalBCSize = bc.size;
        originalBCCenter = bc.center;
        InputActionMap map = inputActions.FindActionMap("SwitchCharacter");
        char1 = map.FindAction("Char1");
        char2 = map.FindAction("Char2");
        char3 = map.FindAction("Char3");

        foreach (GameObject character in characters)
        {
            if (!character.name.ToLower().Equals(activatedCharacter.ToString().ToLower()))
            {
                character.SetActive(false);
            }
        }

        movementScript = GetComponent<Movement>();
        movementScript.UpdateStats(activatedCharacter);
    }

    // Update is called once per frame
    void Update()
    {
        int index = (char1.IsPressed()) ? 0 : (char2.IsPressed()) ? 1 : (char3.IsPressed()) ? 2 : -1;
        if (index >= 0)
        {
            Switch(index);
        }
        
    }

    private void Switch(int index)
    {
        foreach (GameObject character in characters)
        {
            character.SetActive(false);
        }
        if (index >= 0)
        {
            characters[index].SetActive(true);
            activatedCharacter = (SwitchCharacter.ActivatedCharacter)System.Enum.Parse(typeof(ActivatedCharacter), characters[index].name.ToUpper());
            Debug.Log(string.Format("Switched to {0}", activatedCharacter));
            movementScript.UpdateStats(activatedCharacter);
        }

        switch (index) {
            case 1:
                bc.size = new Vector3(bc.size.x, originalBCSize.y * bearColliderHeightMultiplier, bc.size.z);
                bc.center = new Vector3(originalBCCenter.x, originalBCCenter.y + (originalBCSize.y * 2 * (bearColliderHeightMultiplier - 1) / 4 - 0.08f), originalBCCenter.z);
                break;
            default:
                bc.size = originalBCSize;
                bc.center = originalBCCenter;
                break;
        }
    }
}
