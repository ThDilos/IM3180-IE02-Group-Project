using System.Linq;
using TMPro;
using UnityEngine;

interface IInteractable
{
    public void Interact();
    public bool Condition(); // Used to check whether you are in range of triggering (using their w.r.t Trigger Area) and show the floating text
    public Transform getTransform(); // Used to pinpoint the floating interaction text

    public string AlternativeText(); // Alternative texts to fill in the []?
}

public class Interactor : MonoBehaviour
{
    [SerializeField]
    private KeyCode interactionKey = KeyCode.E;

    public Transform interactorSource;
    public float interactionRange; // This range is rather a "Detection" range of interactable object
    // You can set it very big, does not matter. But too small will make interaction not working
    public TMP_Text floatingText;
    private FloatingText floatingTextScript;

    private void Start()
    {
        floatingTextScript = floatingText.GetComponent<FloatingText>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get items in range
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRange);
        var sortedColliders = hitColliders.OrderBy(item => Vector3.Distance(item.transform.position, transform.position));
        foreach (var collider in sortedColliders)
        {
            // Show Interaction Text
            if (collider.gameObject.TryGetComponent(out IInteractable interactObj) && interactObj.Condition())
            {
                string key = interactionKey.ToString();
                string altText = interactObj.AlternativeText();
                if (altText != null) floatingText.text = "[" + key + "]\nTo " + altText;
                else floatingText.text = "[" + key + "]";
                floatingTextScript.target = interactObj.getTransform();
                break; // Break is used so it won't flicker between multiple interactables (Only check for one)
            }
            else
            {
                // When no interactable detected, put it back on player body and hide it by setting contents to nothing
                floatingText.text = "";
                floatingTextScript.target = transform;
            }
        }

        if (Input.GetKeyDown(interactionKey))
        {
            // Get items in range
            hitColliders = Physics.OverlapSphere(transform.position, interactionRange);
            sortedColliders = hitColliders.OrderBy(item => Vector3.Distance(item.transform.position, transform.position));
            foreach (var collider in sortedColliders)
            {
                // If it's interactable, interact with nearest interactable object
                if (collider.gameObject.TryGetComponent(out IInteractable interactObj))
                {
                    if (interactObj.Condition()) // Also check for whether its condition is met (Condition can be whether you are in the setted interaction range, item you are carrying etc. depending on individual Interactables)
                    {
                        interactObj.Interact();
                        break;
                    }
                }
            }
        }
    }
}
