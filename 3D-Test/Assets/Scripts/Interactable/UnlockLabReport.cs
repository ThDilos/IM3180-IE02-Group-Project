using UnityEngine;

public class UnlockLabReport : MonoBehaviour
{
    [SerializeField] GameController gameController;
    public AudioSource audioSource;
    public AudioClip closeBook;
    private void OnDestroy()
    {
        gameController.ObtainLabReport();
        audioSource.PlayOneShot(closeBook, 1.3f);
    }
}
