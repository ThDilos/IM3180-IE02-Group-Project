using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class loreNotes : MonoBehaviour
{
    public GameObject showlore;
    public Image lore1;
    public Image lore2;
    public Image lore3;

    public Button lore1Button;
    public Button lore2Button;
    public Button lore3Button;

    private GameController gameController;

    public static bool shownLore1 = false;
    public static bool shownLore2 = false;
    public static bool shownLore3 = false;

    void Start()
    {
        gameController = GameController.Instance;

        if (showlore)
        {
            showlore.SetActive(false);
        }
        showlore.SetActive(false);
        lore1.enabled = false;
        lore2.enabled = false;
        lore3.enabled = false;
        lore1Button.gameObject.SetActive(false);
        lore2Button.gameObject.SetActive(false);
        lore3Button.gameObject.SetActive(false);

        lore1Button.onClick.AddListener(openlore1);
        lore2Button.onClick.AddListener(openlore2);
        lore3Button.onClick.AddListener(openlore3);

        if (shownLore1) { lore1Button.gameObject.SetActive(true); }
        if (shownLore2) { lore2Button.gameObject.SetActive(true); }
        if (shownLore3) { lore3Button.gameObject.SetActive(true); }
    }
    void Update()
    {
        if (gameController.gotLore1 && !shownLore1) { lore1Button.gameObject.SetActive(true); openlore1(); shownLore1 = true; }
        if (gameController.gotLore2 && !shownLore2) { lore2Button.gameObject.SetActive(true); openlore2(); shownLore2 = true; }
        if (gameController.gotLore3 && !shownLore3) { lore3Button.gameObject.SetActive(true); openlore3(); shownLore3 = true; }

        if (Input.GetMouseButtonDown(0)) // left click
        {
            showlore.transform.SetAsFirstSibling();
            showlore.SetActive(false);
            if (shownLore3)
            {
                SceneManager.LoadScene("Ending Dialogue");
            }
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
