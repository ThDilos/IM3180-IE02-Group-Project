using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Water : MonoBehaviour
{
    // Runtime vars
    private BoxCollider bc;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bc = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] hitColliders = Physics.OverlapBox(transform.position, bc.size);
        foreach (var collider in hitColliders)
        {
            if (collider.gameObject.layer == 0 && collider.gameObject.GetComponent<Rigidbody>() != null)
            {
                Vector3 newVel = collider.gameObject.GetComponent<Rigidbody>().linearVelocity;
                if (newVel.y < 0)
                {
                    collider.gameObject.transform.position = new Vector3(collider.gameObject.transform.position.x, transform.position.y + bc.size.y, collider.gameObject.transform.position.z);
                }
                newVel.y = Mathf.Clamp(newVel.y, 0.0f, 10.0f);
                collider.gameObject.GetComponent<Rigidbody>().linearVelocity = newVel;
            }
            else if (collider.gameObject.CompareTag("Player"))
            {
                collider.gameObject.GetComponent<Movement>().Respawn();
            }
        }
    }
}
