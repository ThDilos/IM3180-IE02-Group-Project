using UnityEngine;

public class FloatingText : MonoBehaviour
{
    Transform mainCam;
    [SerializeField] Transform worldSpaceCanvas;
    public Vector3 targetPos { get; set; }

    // Offset of the text to the object it should be shown upon
    public Vector3 offset = new Vector3(0, 1, 0);

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main.transform;
        // Set its parent to worldSpaceCanvas automatically
        transform.SetParent(worldSpaceCanvas);
    }

    // Update is called once per frame
    void Update()
    {
        if (targetPos != null)
        {
            Vector3 selfPosAdjusted = new Vector3(0, transform.position.y, transform.position.z);
            Vector3 camPosAdjusted = new Vector3(0, mainCam.position.y, mainCam.position.z);

            transform.rotation = Quaternion.LookRotation(selfPosAdjusted - camPosAdjusted); // Look At Camera
            transform.position = targetPos + offset; // Adjust position w.r.t offsets
        }
    }
}
