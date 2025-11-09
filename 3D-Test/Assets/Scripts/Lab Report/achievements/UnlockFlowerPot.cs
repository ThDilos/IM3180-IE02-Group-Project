using UnityEngine;

public class UnlockFlowerPot : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    public AudioSource audioSource;
    public AudioClip potBreakingClip;
    private void OnDestroy()
    {
        gameController.PushFlowerPot();
        if (audioSource == null) return;
        audioSource.PlayOneShot(potBreakingClip);
    }
}
