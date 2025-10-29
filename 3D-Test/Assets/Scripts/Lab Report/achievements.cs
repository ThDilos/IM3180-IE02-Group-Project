using UnityEngine;
using UnityEngine.UI;

public class achievements : MonoBehaviour
{
    public Image badge1;
    public Image badge2;
    public Image badge3;
    public Image badge4;

    [SerializeField] GameController gameController;

    void Start()
    {
        badge1.enabled = false;
        badge2.enabled = false;
        badge3.enabled = false;
        badge4.enabled = false;

    }
    void Update()
    {
        if (gameController.gotKnife) { badge1.enabled = true; }
        if (gameController.gotRubberDuck) { badge2.enabled = true; }
        if (gameController.pushedFlowerPot) { badge3.enabled = true; }
    }
}