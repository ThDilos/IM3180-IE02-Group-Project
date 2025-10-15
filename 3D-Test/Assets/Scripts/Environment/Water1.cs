using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Water1 : MonoBehaviour
{
    [Header("The Mass of RigidBodies able to float on water")]
    [SerializeField] private float floatMass = 1.0f;
    [Header("For Buoyancy")]
    [SerializeField] private float floatForce = 1.0f;

    [Header("Detection Range Setting")]
    [SerializeField] private float detectionSizeToColliderSize = 0.8f;

    // 🔹 Splash Settings (new)
    [Header("Splash Settings")]
    [SerializeField] private AudioSource splashSource;   // assign in Inspector (can be on Water)
    [SerializeField] private AudioClip splashClip;       // splash SFX
    [SerializeField] private float minDownSpeed = -0.1f; // only splash if falling
    [SerializeField] private float splashCooldown = 0.25f;

    private float lastSplashTime;
    public bool GooseInWater { get; private set; } // read-only for other scripts

    // Runtime vars
    private BoxCollider bc;
    private Vector3 checkAreaCenter;
    private Vector3 checkAreaSize;
    private SwitchCharacter sc;
    private Movement movement;

    public bool canFloat;

    // 🔹 Track if a collider was above the water surface last frame (so we can detect re-entry)
    private readonly Dictionary<int, bool> wasAboveSurface = new Dictionary<int, bool>();

    void Start()
    {
        sc = GameObject.Find("Player").GetComponent<SwitchCharacter>();
        movement = sc.transform.GetComponent<Movement>();
        bc = GetComponent<BoxCollider>();
        checkAreaCenter = transform.position + bc.center;
        checkAreaSize = bc.size * detectionSizeToColliderSize;
    }

    void Update()
    {
        Collider[] hitColliders = Physics.OverlapBox(checkAreaCenter, checkAreaSize);
        foreach (var collider in hitColliders)
        {
            Rigidbody rb = null;
            canFloat = false;

            if (collider.CompareTag("Player"))
            {
                if (sc.activatedCharacter != SwitchCharacter.ActivatedCharacter.GOOSE && bc.bounds.Contains(collider.transform.position))
                    movement.Respawn();

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

    // ─────────────────────────────────────────────────────────────────────────────
    // Triggers: enter/exit + re-entry detection while staying inside the trigger
    // ─────────────────────────────────────────────────────────────────────────────

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (sc != null && sc.activatedCharacter == SwitchCharacter.ActivatedCharacter.GOOSE)
        {
            GooseInWater = true;

            // Initialize above/below state at the moment of entering
            float surfaceY = bc.bounds.max.y;               // top face of water volume
            bool currentlyAbove = other.bounds.min.y > surfaceY;
            wasAboveSurface[other.GetInstanceID()] = currentlyAbove;

            // Optional initial splash if falling into water on first enter
            var rb = other.attachedRigidbody ?? sc.transform.GetComponent<Rigidbody>();
            bool fallingEnough = (rb == null) || (rb.linearVelocity.y <= minDownSpeed);
            if (!currentlyAbove && fallingEnough) TryPlaySplashAt(other.ClosestPoint(bc.bounds.center));
        }
        else
        {
            // Not goose: your original respawn behavior if inside bounds
            if (bc.bounds.Contains(other.transform.position))
                movement.Respawn();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (sc == null || sc.activatedCharacter != SwitchCharacter.ActivatedCharacter.GOOSE) return;

        // While inside the trigger, detect downward crossing of the water surface:
        // If goose jumps up above the surface and then comes down below the surface again,
        // we play a splash on that downward crossing.
        float surfaceY = bc.bounds.max.y;     // assumes water surface is the top of the BoxCollider
        int id = other.GetInstanceID();

        bool prevAbove = true;
        wasAboveSurface.TryGetValue(id, out prevAbove);

        // Use the lowest point of the player's collider as a simple "feet" proxy
        bool nowAbove = other.bounds.min.y > surfaceY;

        // Detect downward crossing (prev above, now below) → splash
        if (prevAbove && !nowAbove)
        {
            var rb = other.attachedRigidbody ?? sc.transform.GetComponent<Rigidbody>();
            bool fallingEnough = (rb == null) || (rb.linearVelocity.y <= minDownSpeed);
            if (fallingEnough) TryPlaySplashAt(other.ClosestPoint(bc.bounds.center));
        }

        // Update state
        wasAboveSurface[id] = nowAbove;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        wasAboveSurface.Remove(other.GetInstanceID());

        if (sc != null && sc.activatedCharacter == SwitchCharacter.ActivatedCharacter.GOOSE)
        {
            GooseInWater = false;
        }
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────────────────────────────────────

    private void TryPlaySplashAt(Vector3 pos)
    {
        if (splashClip == null) return;
        if (Time.time - lastSplashTime <= splashCooldown) return;

        lastSplashTime = Time.time;

        if (splashSource != null)
            splashSource.PlayOneShot(splashClip);
        else
            AudioSource.PlayClipAtPoint(splashClip, pos);
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
