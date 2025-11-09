using UnityEngine;

public class CollectKey : MonoBehaviour
{
    [SerializeField] private TriggerDialog dialogTrigger;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip keyPickUp;

    private void Update()
    {
        if (dialogTrigger != null && dialogTrigger.interacting)
        {
            audioSource.PlayOneShot(keyPickUp);
        }
    }
}
