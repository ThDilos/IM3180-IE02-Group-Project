using Unity.VisualScripting;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Framing")]
    [SerializeField] public Vector2 offset = new Vector2(0f, 1.5f);
    [Tooltip("How far to look ahead based on the player velocity (sec). 0 = off")]
    [SerializeField] float lookAheadTime = 0.25f;

    [Header("Dead Zone")]
    [SerializeField] bool useDeadZone = true;
    [SerializeField] Vector2 deadZoneSize = new Vector2(3.0f, 2.0f);
    [SerializeField] bool drawDeadZone = true;

    [Header("Extras")]
    [SerializeField] float scrollSensitivity = 10f;

    [Header("Distance Clamper")]
    [Tooltip("Restrict Camera's Distance to Player")]
    [SerializeField] bool enableDistanceClamper = true;
    [Tooltip("x is Min, y is Max")]
    [SerializeField] Vector2 clampCamDistance = new Vector2(10, 20);
    [Tooltip("Only Used for Cam Clamping. How fast does camera follow?")]
    [SerializeField] float camFollowSpeed = 1f;

    Vector3 velocity;
    Rigidbody targetRB;
    Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        if (target)
        {
            targetRB = target.GetComponent<Rigidbody>();
        }
    }

    private void Update()
    {
        if (Input.mouseScrollDelta.y > 0)
        {
            transform.position += transform.forward * scrollSensitivity * Time.deltaTime;
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            transform.position -= transform.forward * scrollSensitivity * Time.deltaTime;
        }
    }

    // Called every frame but it's the last to be executed
    private void LateUpdate()
    {
        if (!target) return;

        // Desired point: target + offset + velocity look ahead
        Vector2 look = Vector2.zero;

        if (targetRB)
        {
            look = targetRB.linearVelocity * lookAheadTime;
        }

        Vector3 focus = new Vector3(
            target.position.x + offset.x + look.x,
            target.position.y + offset.y + look.y,
            transform.position.z
            );

        Vector3 currentPosition = transform.position;
        Vector3 desiredPoint;

        if (useDeadZone)
        {
            // Move only enough to push the focus point back inside the dead zone
            Vector3 half = deadZoneSize * 0.5f;

            float dx = 0;
            float dy = 0;

            float deltaX = focus.x - currentPosition.x;
            float deltaY = focus.y - currentPosition.y;

            if (deltaX > half.x)
            {
                dx = deltaX - half.x;
            }
            if (deltaX < -half.x)
            {
                dx = deltaX + half.x;
            }
            if (deltaY > half.y)
            {
                dy = deltaY - half.y;
            }
            if (deltaY < -half.y)// yipeeeeeeeeee
            {
                dy = deltaY + half.y;
            }

            desiredPoint = new Vector3(currentPosition.x + dx, currentPosition.y + dy, currentPosition.z);
        }
        else
        {
            desiredPoint = focus;
        }

        // Find the next using damping
        Vector3 next = Vector3.SmoothDamp(transform.position, desiredPoint, ref velocity, Time.unscaledDeltaTime);

        // Set the camera position
        transform.position = next;

        // Clamp Cam Distance To Player
        if (enableDistanceClamper)
        {
            float toTargetDistance = (transform.position - target.transform.position).magnitude;
            Vector3 toTargetDirection = (transform.position - target.transform.position).normalized;
            toTargetDirection.Scale(new Vector3(1, 0, 1)); // No Y Change
            if (toTargetDistance < clampCamDistance.x)
            {
                transform.position += toTargetDirection * camFollowSpeed * Time.deltaTime;
            }
            else if (toTargetDistance > clampCamDistance.y)
            {
                transform.position -= toTargetDirection * camFollowSpeed * Time.deltaTime;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawDeadZone || !useDeadZone) return;

        Gizmos.color = Color.cyan;
        Vector3 cube = transform.position;

        Vector3 size = new Vector3(deadZoneSize.x, deadZoneSize.y, 0);
        Gizmos.DrawWireCube(cube, size);
    }
}
