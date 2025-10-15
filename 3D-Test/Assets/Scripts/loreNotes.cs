using UnityEngine;
using UnityEngine.UI;

public class loreNotes : MonoBehaviour
{
    public GameObject showlore;
    public Image lore1;
    public Image lore2;
    public Image lore3;

    public Button lore1Button;
    public Button lore2Button;
    public Button lore3Button;

    [SerializeField] GameController gameController;

    void Start()
    {
        if (showlore)
        {
            showlore.SetActive(false);
        }
        showlore.SetActive(false);
        lore1.enabled = false;
        lore2.enabled = false;
        lore3.enabled = false;
        lore1Button.gameObject.SetActive(false);
        lore2Button.gameObject.SetActive(true);
        lore3Button.gameObject.SetActive(false);

        if (gameController.gotLore1) { lore1Button.gameObject.SetActive(true); showlore.SetActive(true);  lore1.enabled = true; }
        if (gameController.gotLore2) { lore1Button.gameObject.SetActive(true); showlore.SetActive(true); lore2.enabled = true; }
        if (gameController.gotLore3) { lore1Button.gameObject.SetActive(true); showlore.SetActive(true); lore3.enabled = true; }

        lore1Button.onClick.AddListener(openlore1);
        lore2Button.onClick.AddListener(openlore2);
        lore3Button.onClick.AddListener(openlore3);
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // left click
        {
            showlore.transform.SetAsFirstSibling();
            showlore.SetActive(false);
        }
    }
    public void openlore1()
    {
        showlore.transform.SetAsLastSibling();
        showlore.SetActive(true);
        lore1.enabled = true;
        lore2.enabled = false;
        lore3.enabled = false;
    }
    public void openlore2()
    {
        showlore.transform.SetAsLastSibling();
        showlore.SetActive(true);
        lore1.enabled = false;
        lore2.enabled = true;
        lore3.enabled = false;
    }
    public void openlore3()
    {
        showlore.transform.SetAsLastSibling();
        showlore.SetActive(true);
        lore1.enabled = false;
        lore2.enabled = false;
        lore3.enabled = true;
    }
}
