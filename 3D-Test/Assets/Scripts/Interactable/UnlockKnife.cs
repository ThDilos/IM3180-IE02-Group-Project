using UnityEngine;

public class UnlockKnife : MonoBehaviour
{
    [SerializeField] GameController gameController;
    public AudioSource audioSource;
    public AudioClip knife;
    private void OnDestroy()
    {
        gameController.ObtainKnife();
        audioSource.PlayOneShot(knife, 1.3f);
    }
}
