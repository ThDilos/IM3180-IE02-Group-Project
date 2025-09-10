using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    Transform mainCam;
    [SerializeField] Transform worldSpaceCanvas;
    private TMP_Text text;
    public Transform target { get; set; }
    [SerializeField] private float hideDistanceModifier = 5.0f; // How long should the invisibilification be?

    // Offset of the text to the object it should be shown upon
    public Vector3 offset = new Vector3(0, 1, 0);

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main.transform;
        text = GetComponent<TMP_Text>();
        // Set its parent to worldSpaceCanvas automatically
        transform.SetParent(worldSpaceCanvas);
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - mainCam.transform.position); // Look At Camera
            transform.position = target.position + offset; // Adjust position w.r.t offsets
        }
        float camCloseness = Vector3.Distance(Camera.main.transform.position, transform.position);
        camCloseness = (camCloseness - 2.0f) / hideDistanceModifier;
        if (text != null)
        {
            text.alpha = Mathf.Clamp(camCloseness, 0.0f, 1.0f);
        }
    }
}
