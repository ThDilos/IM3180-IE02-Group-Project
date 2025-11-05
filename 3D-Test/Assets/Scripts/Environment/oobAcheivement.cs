using UnityEngine;

public class oobAcheivement : MonoBehaviour
{
    [SerializeField] GameController gameController;
    [SerializeField] Water waterScriptOnSkylimit;
    private void Update()
    {
        if (waterScriptOnSkylimit.OnceTouchedFired)
        {
            gameController.oob();
        }
    }
}
