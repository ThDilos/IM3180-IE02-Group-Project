using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Water : MonoBehaviour
{
    [Header("The Mass of RigidBodies able to float on water")]
    [SerializeField] private float floatMass = 1.0f;
    [Header("For Buoyancy")]
    [SerializeField] private float floatForce = 1.0f;

    [Header("Detection Range Setting")]
    [SerializeField] private float detectionSizeToColliderSize = 0.8f;
    // Runtime vars
    private BoxCollider bc;
    private Vector3 checkAreaCenter;
    private Vector3 checkAreaSize;
    private SwitchCharacter sc;
    private Movement movement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sc = GameObject.Find("Player").GetComponent<SwitchCharacter>();
        movement = sc.transform.GetComponent<Movement>();
        bc = GetComponent<BoxCollider>();
        checkAreaCenter = transform.position + bc.center;
        checkAreaSize = bc.size * detectionSizeToColliderSize;
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] hitColliders = Physics.OverlapBox(checkAreaCenter, checkAreaSize);
        foreach (var collider in hitColliders)
        {
            Rigidbody rb = null;
            bool canFloat = false;

            if (collider.CompareTag("Player"))
            {
                if (sc.activatedCharacter != SwitchCharacter.ActivatedCharacter.GOOSE && bc.bounds.Contains(collider.transform.position)) movement.Respawn();

                rb = sc.transform.GetComponent<Rigidbody>();
                canFloat = true;
            }
            else
            {
                if (collider.gameObject.GetComponent<Rigidbody>() == null) continue;

                rb = collider.gameObject.GetComponent<Rigidbody>();
                canFloat = floatMass > rb.mass;
            }
            // Skip the float mechanics for now
            continue;

            // Dont know if working at all
            if (canFloat)
            {
                Vector3 newVel = rb.linearVelocity;

                rb.AddForce(Vector3.up * floatForce / 100.0f, ForceMode.Force);
                newVel.y = Mathf.Clamp(newVel.y, 0.0f, 10.0f);
                rb.linearVelocity = newVel;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (bc != null && checkAreaCenter != null && checkAreaSize != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(checkAreaCenter, checkAreaSize);
        }
    }
}
