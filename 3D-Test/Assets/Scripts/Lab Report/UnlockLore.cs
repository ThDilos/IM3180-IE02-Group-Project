using UnityEngine;
using System.Collections;

public class UnlockLore : MonoBehaviour
{
    [SerializeField] private TriggerDialog dialogTrigger;

    [Header("Times Player has been in Lab")]
    [SerializeField] private string roomId = "Lab";
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float reenterCooldown = 0.5f;
    
    private float lastEnterTime = -999f;

    // latches so each visit’s reward happens once only
    private bool gaveLore1, gaveLore2, gaveLore3;
    private GameController gameController;

    private void Start()
    {
        gameController = GameController.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (Time.time - lastEnterTime < reenterCooldown) return; // debounce quick re-entries
        lastEnterTime = Time.time;

        //int visit = gameController.EnterRoom(roomId);
        int visit = gameController.timesInLab;

        switch (visit)
        {
            case 1:
                if (!gaveLore1) StartCoroutine(GiveWhenInteracting(visit));
                break;
            case 2:
                if (!gaveLore2) StartCoroutine(GiveWhenInteracting(visit));
                break;
            case 3:
                if (!gaveLore3) StartCoroutine(GiveWhenInteracting(visit));
                break;
            default:
                break;
        }
    }

    private IEnumerator GiveWhenInteracting(int visit)
    {
        // optional safety timeout so we don't wait forever if player walks away
        const float timeout = 6f;
        float t = 0f;

        // Wait until the dialog is actually open this session
        yield return new WaitUntil(() =>
        {
            t += Time.deltaTime;
            return (dialogTrigger != null && dialogTrigger.interacting) || t >= timeout;
        });

        // If it did open, award once
        if (dialogTrigger != null && dialogTrigger.interacting)
        {
            switch (visit)
            {
                case 1:
                    if (!gaveLore1) { gameController.ObtainLore1(); gaveLore1 = true; Debug.Log("Showed lore1"); }
                    break;
                case 2:
                    if (!gaveLore2) { gameController.ObtainLore2(); gaveLore2 = true; Debug.Log("Showed lore2"); }
                    break;
                case 3:
                    if (!gaveLore3) { gameController.ObtainLore3(); gaveLore3 = true; Debug.Log("Showed lore3"); }
                    break;
            }
        }
    }
}
