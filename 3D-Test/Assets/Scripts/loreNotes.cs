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

    // Update is called once per frame
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
