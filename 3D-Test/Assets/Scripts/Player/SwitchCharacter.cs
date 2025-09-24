using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]

public class SwitchCharacter : MonoBehaviour
{
    public enum ActivatedCharacter 
    {
        GOOSE,
        BEAR,
        CAT
    }

    public class IndividualCharacter : MonoBehaviour
    {
        private BoxCollider bc;
        private GameObject sprite;

        private void Awake()
        {
            bc = transform.Find("Collider").GetComponent<BoxCollider>();
            sprite = transform.Find("Sprite").gameObject;
        }

        public BoxCollider GetBoxCollider()
        {
            return bc;
        }

        public GameObject GetSpriteObject()
        {
            return sprite;
        }
        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }

    [Tooltip("Which Character the player will start as?")]
    [SerializeField] public ActivatedCharacter activatedCharacter;
    [Header("Put In the Character GameObject")]
    [Tooltip("Each MUST contain a \"Collider\" with a BoxCollider, and a \"Sprite\" contains the sprite renderer. The Order of them will affect which key switch to which character")]
    [SerializeField] public GameObject[] characters;
    public InputActionAsset inputActions;

    private InputAction char1;
    private InputAction char2;
    private InputAction char3;

    public Dictionary<ActivatedCharacter, IndividualCharacter> characterMap = new Dictionary<ActivatedCharacter, IndividualCharacter>();

    private Movement movementScript;

    private Transform[] allSpriteTransforms;

    private void Awake()
    {
        InputActionMap map = inputActions.FindActionMap("SwitchCharacter");
        char1 = map.FindAction("Char1");
        char2 = map.FindAction("Char2");
        char3 = map.FindAction("Char3");

        allSpriteTransforms = new Transform[characters.Length];

        int index = 0;
        foreach (GameObject character in characters)
        {
            IndividualCharacter newCharacter = character.AddComponent<IndividualCharacter>();

            ActivatedCharacter tempEnum;

            if (character.name == "Bear") tempEnum = ActivatedCharacter.BEAR;
            else if (character.name == "Goose") tempEnum = ActivatedCharacter.GOOSE;
            else tempEnum = ActivatedCharacter.CAT;

            allSpriteTransforms[index] = newCharacter.GetSpriteObject().transform;
            characterMap.Add(tempEnum, newCharacter);
            index++;
        }
        foreach (GameObject character in characters)
        {
            character.SetActive(false);
        }

        characterMap[activatedCharacter].SetActive(true);

        movementScript = GetComponent<Movement>();
        movementScript.UpdateStats(activatedCharacter);
    }

    // Update is called once per frame
    void Update()
    {
        int index = (char1.WasPerformedThisFrame()) ? 0 :
            (char2.WasPerformedThisFrame()) ? 1 :
            (char3.WasPerformedThisFrame()) ? 2 : -1;
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
            activatedCharacter = (ActivatedCharacter)System.Enum.Parse(typeof(ActivatedCharacter), characters[index].name.ToUpper());
            Debug.Log(string.Format("Switched to {0}", activatedCharacter));
            movementScript.UpdateStats(activatedCharacter);
        }
    }

    public BoxCollider GetActiveBoxCollider()
    {
        return characterMap[activatedCharacter].GetBoxCollider();
    }

    public Transform[] GetAllSpriteTransforms()
    {
        return allSpriteTransforms;
    }
}
