using UnityEngine;
using UnityEngine.UI;

public class achievements : MonoBehaviour
{
    public Image knife;
    public Image rubberDuck;
    //public Image ach3;

    [SerializeField] GameController gameController;

    void Start()
    {
        knife.enabled = false;
        rubberDuck.enabled = false;
        //ach3.enabled = false;

    }
    void Update()
    {
        if (gameController.gotKnife) { knife.enabled = true; }
        if (gameController.gotRubberDuck) { rubberDuck.enabled = true; }
        //if (gameController.gotLore3) { lore3Button.gameObject.SetActive(true); openlore3(); shownLore3 = true; }
    }
}