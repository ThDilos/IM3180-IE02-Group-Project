using UnityEngine;

public class UnlockKnife : MonoBehaviour
{
    [SerializeField] GameController gameController;
    public AudioSource audioSource;
    public AudioClip knife;
    private void OnDestroy()
    {
        gameController.ObtainKnife();
        if (audioSource == null) return;
        audioSource.PlayOneShot(knife, 1.3f);
    }
}
