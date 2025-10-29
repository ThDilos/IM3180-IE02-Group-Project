using UnityEngine;

public class UnlockRubberDuck : MonoBehaviour
{
    [SerializeField] GameController gameController;
    public AudioSource audioSource;
    public AudioClip rubberDuckCLip;
    private void OnDestroy()
    {
        gameController.ObtainRubberDuck();
        audioSource.PlayOneShot(rubberDuckCLip);
    }
}
