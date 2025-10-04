using UnityEngine;

public class UnlockLabReport : MonoBehaviour
{
    [SerializeField] GameController gameController;
    private void OnDestroy()
    {
        gameController.ObtainLabReport();
    }
}
