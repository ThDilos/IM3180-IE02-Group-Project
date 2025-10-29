using UnityEngine;

public class UnlockFlowerPot : MonoBehaviour
{
    [SerializeField] GameController gameController;
    public AudioSource audioSource;
    public AudioClip potBreakingClip;
    private void OnDestroy()
    {
        gameController.PushFlowerPot();
        audioSource.PlayOneShot(potBreakingClip);
    }
}
