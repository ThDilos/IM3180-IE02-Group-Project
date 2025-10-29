using UnityEngine;

public class UnlockLabReport : MonoBehaviour
{
    [SerializeField] GameController gameController;
    public AudioSource audioSource;
    public AudioClip closeBookClip;
    private void OnDestroy()
    {
        gameController.ObtainLabReport();
        audioSource.PlayOneShot(closeBookClip);
    }
}
