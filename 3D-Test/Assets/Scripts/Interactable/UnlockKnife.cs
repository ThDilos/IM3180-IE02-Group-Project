using UnityEngine;

public class UnlockKnife : MonoBehaviour
{
    [SerializeField] GameController gameController;
    public AudioSource audioSource;
    public AudioClip knife;
    private void OnDestroy()
    {
        gameController.ObtainLore1();
        audioSource.PlayOneShot(knife, 1.3f);
    }
}
