using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

[RequireComponent(typeof(BoxCollider))]
public class Water : MonoBehaviour
{
    [Header("Detection Range Setting")]
    [SerializeField] private float detectionSizeToColliderSize = 0.8f;

    [Header("Liquid Setting. Used for Buoyancy and Drag Force")]
    [Tooltip("kg/m^3")]
    [SerializeField] private float liquidDensity = 1000.0f;
    [Tooltip("m/s")]
    [SerializeField] private Vector3 flowVelocity = Vector3.zero;

    [Header("Extra Amendments, for when real values bring too strong of an effect")]
    [SerializeField] private int buoyancyDivisionScale = 50;
    [SerializeField] private int dragDivisionScale = 1000;


    private static float dragCoefficient = 2.05f;

    // IMPORTANT:
    // SET UP LIQUID AS A BOX, WITH BOX COLLIDER BEING THE LIQUID ITSELF!!! (Do not put collider in the air, match it perfectly to the 3D aspect of where the liquid should be)


    // Liquid Drag Force F = C * A * (liquidDensity * V^2) / 2
    // Where: C is the drag coefficient, we'll use 2.05 here (for square)
    // A is the reference area, since we are using box collider, it's the area of one side (We'll use the left/right side for convience)
    // V is the relative velocity of the object to the liquid

    // Liquid Buoyancy F = liquidDensity * g * Volume of the liquid displaced
    // g can be get from physics.gravity
    // Since calculating the exact volume of collider inside liquid is very difficult, we will use base area * y cords difference instead, clamped to collider's height
    // F = liquidDensity * g * (collider base area) * Min(collider.transform.y - water surface y, collider.size.y)

    // Runtime vars
    private BoxCollider bc;
    private Vector3 checkAreaCenter;
    private Vector3 checkAreaSize;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
            // Ignore everything without a mass setting
            // Note, some objects' rigid body is on its parent somehow! ts pmo sm ngl but fixable :3
            if (collider.transform.GetComponentInParent<Rigidbody>() == null && collider.transform.GetComponent<Rigidbody>() == null) continue;

            Rigidbody rb = null;

            if (collider.transform.GetComponentInParent<Rigidbody>() == null)
            {
                rb = collider.GetComponent<Rigidbody>();
            }
            else
            {
                rb = collider.transform.GetComponentInParent<Rigidbody>();
            }

            if (collider.CompareTag("Player"))
            {
                SwitchCharacter sc = collider.GetComponentInParent<SwitchCharacter>();
                Movement movement = sc.GetComponentInParent<Movement>();

                // Respawn player if not goose
                if (sc.activatedCharacter != SwitchCharacter.ActivatedCharacter.GOOSE)
                {
                    movement.Respawn();
                    continue;
                }
            }

            rb.AddForce(GetBuoyancy(collider) + GetDrag(collider, rb), ForceMode.Force);
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

    // Return the magnitude * direction of buoyancy, normally it's just (0, Float Force, 0)
    private Vector3 GetBuoyancy(Collider collider)
    {
        Vector3 buoyancy = Vector3.zero;
        float gravityConstant = Physics.gravity.y; // Fetch gravity constant from physics engine
        // Volume of Liquid Displaced = collider base area * Min(collider-liquid surface y difference, collider height)
        float colliderBottomY = collider.transform.position.y;
        float waterSurfaceY = transform.position.y + bc.size.y;
        float displacedLiquidVolume = collider.bounds.size.x * collider.bounds.size.z * Mathf.Min(waterSurfaceY - colliderBottomY, -collider.bounds.size.y);

        buoyancy = liquidDensity * gravityConstant * displacedLiquidVolume * Vector3.up / buoyancyDivisionScale;
        return buoyancy;
    }


    // Returns the magnitude * direction of water drag.
    private Vector3 GetDrag(Collider collider, Rigidbody rb)
    {
        Vector3 drag = Vector3.zero;
        Vector3 colliderVel = rb.linearVelocity;

        float relativeVelocity = Vector3.Magnitude(colliderVel + flowVelocity);
        float referenceArea = collider.bounds.size.y * collider.bounds.size.z;

        drag = dragCoefficient * (liquidDensity / dragDivisionScale * relativeVelocity * relativeVelocity) * referenceArea / 2 * (-colliderVel.normalized); // Drag against movement direction
        return drag;
    }
}
