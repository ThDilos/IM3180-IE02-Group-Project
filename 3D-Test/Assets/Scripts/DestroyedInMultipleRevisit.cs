using UnityEngine;

public class DestroyedInMultipleRevisit : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GameController.Instance.timesInLab > 1) Destroy(gameObject);
    }
}
