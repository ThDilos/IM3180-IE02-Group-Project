using UnityEngine;

public class UnlockLabReport : MonoBehaviour
{
    [SerializeField] GameController gameController;
    public AudioSource audioSource;
    public AudioClip closeBookClip;
    private void OnDestroy()
    {
        if (GameController.Instance == null) return;

        gameController.ObtainLabReport();
        audioSource.PlayOneShot(closeBookClip);
    }
}
